import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Team, TeamSearch } from '@/app/api/models/team.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamCard, TeamRole } from '@/app/shared/components/cards/team-card/team-card';
import { AuthStateService } from '@/app/store/AuthStateService';
import { TeamSearchComponent } from '@/app/shared/components/search';
import { ConfirmDeleteModal } from '@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal';

@Component({
  selector: 'app-user-teams',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    LucideAngularModule,
    TeamCard,
    TeamSearchComponent,
    ConfirmDeleteModal
  ],
  templateUrl: './user-teams.html',
  styleUrl: './user-teams.css'
})
export class UserTeams implements OnInit {
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private authStateStore = inject(AuthStateService);
  
  readonly icons = LucideIcons;
  readonly Math = Math;
  
  teams = signal<Team[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = signal(6);
  totalPages = signal(0);
  isLoading = signal(false);
  searchCriteria = signal<Partial<TeamSearch>>({});
  showDeleteModal = signal(false);
  isDeleting = signal(false);
  teamToDelete = signal<Team | null>(null);

  getTeamRole(): TeamRole {
    return 'Manager';
  }

  ngOnInit() {
    this.loadMyTeams();
  }

  loadMyTeams() {
    this.isLoading.set(true);
    
    const searchParams = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      ...this.searchCriteria()
    };
    
    this.teamService.getTeams(searchParams).subscribe({
      next: (response: any) => {
        this.teams.set(response.data);
        this.totalCount.set(response.totalRecords);
        this.totalPages.set(response.totalPages);
      },
      error: (error: any) => {
        this.isLoading.set(false);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  onSearchChange(searchCriteria: Partial<TeamSearch>) {
    this.searchCriteria.set(searchCriteria);
    this.currentPage.set(1); 
    this.loadMyTeams();
  }

  onClearSearch() {
    this.searchCriteria.set({});
    this.currentPage.set(1);
    this.loadMyTeams();
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadMyTeams();
    }
  }

  onPageChange(page: number) {
    this.goToPage(page);
  }

  onPageSizeSelectChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    const newPageSize = parseInt(target.value);
    this.pageSize.set(newPageSize);
    this.currentPage.set(1);
    this.loadMyTeams();
  }

  hasPreviousPage(): boolean {
    return this.currentPage() > 1;
  }

  hasNextPage(): boolean {
    return this.currentPage() < this.totalPages();
  }

  previousPage() {
    if (this.hasPreviousPage()) {
      this.onPageChange(this.currentPage() - 1);
    }
  }

  nextPage() {
    if (this.hasNextPage()) {
      this.onPageChange(this.currentPage() + 1);
    }
  }

  getPageNumbers(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: number[] = [];
    
    if (total <= 7) {
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      const start = Math.max(1, current - 2);
      const end = Math.min(total, current + 2);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
    }
    
    return pages;
  }

  get pageNumbers(): number[] {
    return this.getPageNumbers();
  }

  onTeamEdit(team: Team) {
    this.router.navigate(['/user/teams', team.id, 'edit']);
  }

  onTeamView(team: Team) {
    this.router.navigate(['/user/teams', team.id]);
  }

  onTeamDelete(teamId: string) {
    const team = this.teams().find(t => t.id === teamId);
    if (!team) return;
    
    this.teamToDelete.set(team);
    this.showDeleteModal.set(true);
  }

  closeDeleteModal() {
    this.showDeleteModal.set(false);
    this.teamToDelete.set(null);
  }

  confirmDelete() {
    const team = this.teamToDelete();
    if (!team) return;

    this.isDeleting.set(true);
    this.teamService.deleteTeam(team.id).subscribe({
      next: () => {
        this.toastService.success('Team deleted successfully');
        this.closeDeleteModal();
        this.loadMyTeams();
      },
      error: (error) => {
        this.toastService.error('Failed to delete team');
        this.isDeleting.set(false);
      }
    });
  }
}