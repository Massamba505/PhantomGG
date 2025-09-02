import { Team, TeamSearchRequest } from '@/app/shared/models/tournament';
import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TeamCard } from '@/app/shared/components/team-card/team-card';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastService } from '@/app/shared/services/toast.service';
import { TeamService } from '@/app/core/services/team.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from "lucide-angular";
import { ConfirmDeleteModal } from "@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal";

@Component({
  selector: 'app-teams',
  templateUrl: './teams.html',
  styleUrls: ['./teams.css'],
  standalone: true,
  imports: [
    DashboardLayout,
    CommonModule,
    FormsModule,
    LucideAngularModule,
    TeamCard,
    ConfirmDeleteModal
],
})

export class Teams implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toast = inject(ToastService);
  private teamService = inject(TeamService);
  
  searchTerm = '';
  isDeleteModalOpen = signal(false);
  deletingTeam = signal<string | null>(null);
  readonly icons = LucideIcons;
  gridLayout = signal<boolean>(true);
  loading = signal<boolean>(false);

  teams = signal<Team[]>([]);

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.searchTerm = params['searchTerm'] || '';
      
      if (this.searchTerm) {
        this.searchTeams();
      } else {
        this.loadAllTeams();
      }
    });
  }

  async loadAllTeams() {
    try {
      this.loading.set(true);
      const response = await this.teamService.getAllTeams().toPromise();
      this.teams.set(response?.data || []);
    } catch (error) {
      console.error('Failed to load teams:', error);
      this.toast.error('Failed to load teams');
      this.teams.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  async searchTeams() {
    try {
      this.loading.set(true);
      const searchRequest: TeamSearchRequest = {
        searchTerm: this.searchTerm || undefined
      };
      
      const response = await this.teamService.searchTeams(searchRequest).toPromise();
      this.teams.set(response?.data || []);
    } catch (error) {
      console.error('Failed to search teams:', error);
      this.toast.error('Failed to search teams');
      this.teams.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  onSearchChange() {
    this.updateUrlAndLoad();
  }

  private updateUrlAndLoad() {
    const queryParams: any = {};
    
    if (this.searchTerm) {
      queryParams.searchTerm = this.searchTerm;
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'replace'
    });

  }

  handleEditTeam(team: Team) {
    this.toast.info('Edit team functionality not implemented yet');
  }

  switchLayout(toGrid: boolean) {
    this.gridLayout.set(toGrid);
  }

  handleDeleteTeam(id: string) {
    this.deletingTeam.set(id);
    this.isDeleteModalOpen.set(true);
  }

  async confirmDelete() {
    const teamId = this.deletingTeam();
    if (!teamId) return;

    try {
      await this.teamService.deleteTeam(teamId).toPromise();
      
      const currentTeams = this.teams();
      const updatedTeams = currentTeams.filter((t: Team) => t.id !== teamId);
      this.teams.set(updatedTeams);
      
      this.toast.success('Team deleted successfully');
    } catch (error) {
      console.error('Failed to delete team:', error);
      this.toast.error('Failed to delete team');
    } finally {
      this.isDeleteModalOpen.set(false);
      this.deletingTeam.set(null);
    }
  }

  handleViewTeam(team: Team) {
    this.router.navigate(['/tournaments', 'details', team.tournamentId]);
  }

  createNewTeam() {
    this.toast.info('Create team functionality not implemented yet');
  }
}
