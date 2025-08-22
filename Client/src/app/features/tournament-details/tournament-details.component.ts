import { Team, TeamFormData, Tournament } from '@/app/shared/models/tournament';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ModalComponent } from '@/app/shared/components/modal/modal.component';
import { TeamFormComponent } from '@/app/shared/components/team-form/team-form.component';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';

@Component({
  selector: 'app-tournament-details',
  templateUrl: './tournament-details.component.html',
  styleUrls: ['./tournament-details.component.css'],
  standalone: true,
  imports: [ModalComponent, TeamFormComponent, DashboardLayout],
})
export class TournamentDetailsComponent implements OnInit {
  sidebarOpen = false;
  isAddTeamModalOpen = false;
  isEditTeamModalOpen = false;
  editingTeam: Team | null = null;
  tournament: Tournament | null = null;

  statusColors = {
    draft: 'bg-muted text-muted-foreground',
    active: 'bg-primary text-primary-foreground',
    completed: 'bg-success text-success-foreground',
  };

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.tournament = {
      id: id || '1',
      name: 'Summer League 2024',
      description:
        'Annual summer soccer tournament for youth teams across the region. This prestigious competition brings together the best young talent from across the country to compete in a month-long championship.',
      startDate: '2024-06-15',
      endDate: '2024-07-30',
      maxTeams: 16,
      status: 'active',
      createdAt: '2024-05-01',
      teams: [
        {
          id: '1',
          name: 'FC Barcelona Academy',
          city: 'Barcelona',
          coach: 'Carlos Rodriguez',
          players: 22,
          tournamentId: id || '1',
          createdAt: '2024-05-15',
        },
        {
          id: '2',
          name: 'Real Madrid Youth',
          city: 'Madrid',
          coach: 'Miguel Santos',
          players: 20,
          tournamentId: id || '1',
          createdAt: '2024-05-16',
        },
        {
          id: '3',
          name: 'Manchester City U18',
          city: 'Manchester',
          coach: 'James Wilson',
          players: 21,
          tournamentId: id || '1',
          createdAt: '2024-05-17',
        },
      ],
    };
  }

  getTournamentDuration(startDate: string, endDate: string): number {
    const start = new Date(startDate);
    const end = new Date(endDate);
    return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
  }

  handleAddTeam(formData: TeamFormData) {
    if (!this.tournament) return;

    const newTeam: Team = {
      id: Date.now().toString(),
      ...formData,
      tournamentId: this.tournament.id,
      createdAt: new Date().toISOString(),
    };

    this.tournament = {
      ...this.tournament,
      teams: [...this.tournament.teams, newTeam],
    };
    this.isAddTeamModalOpen = false;
  }

  handleEditTeam(team: Team) {
    this.editingTeam = team;
    this.isEditTeamModalOpen = true;
  }

  handleUpdateTeam(formData: TeamFormData) {
    if (!this.tournament || !this.editingTeam) return;

    this.tournament = {
      ...this.tournament,
      teams: this.tournament.teams.map((team) =>
        team.id === this.editingTeam!.id ? { ...team, ...formData } : team
      ),
    };
    this.isEditTeamModalOpen = false;
    this.editingTeam = null;
  }

  handleDeleteTeam(teamId: string) {
    if (!this.tournament) return;

    if (
      confirm('Are you sure you want to remove this team from the tournament?')
    ) {
      this.tournament = {
        ...this.tournament,
        teams: this.tournament.teams.filter((team) => team.id !== teamId),
      };
    }
  }
}
