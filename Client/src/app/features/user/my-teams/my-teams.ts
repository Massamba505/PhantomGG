import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserLayout } from '../../../shared/components/layouts/user-layout/user-layout';

interface Team {
  id: number;
  name: string;
  description: string;
  logo: string;
  logoUrl?: string;
  memberCount: number;
  maxMembers: number;
  numberOfPlayers: number;
  role: 'captain' | 'member' | 'pending';
  status: 'active' | 'inactive' | 'disbanded' | 'pending';
  joinedDate: Date;
  createdAt: Date;
  wins: number;
  losses: number;
  tournaments: number;
  manager?: string;
  rank?: number;
  currentTournament?: boolean;
  tournamentName?: string;
}

@Component({
  selector: 'app-my-teams',
  standalone: true,
  imports: [CommonModule, RouterModule, UserLayout],
  templateUrl: './my-teams.component.html',
  styleUrl: './my-teams.component.css'
})
export class MyTeams {
  private allTeams = signal<Team[]>([
    {
      id: 1,
      name: 'Shadow Runners',
      description: 'Competitive esports team focused on strategic gameplay',
      logo: '/assets/team-logos/shadow-runners.png',
      logoUrl: '/assets/team-logos/shadow-runners.png',
      memberCount: 5,
      maxMembers: 6,
      numberOfPlayers: 5,
      role: 'captain',
      status: 'active',
      joinedDate: new Date('2024-01-15'),
      createdAt: new Date('2024-01-15'),
      wins: 15,
      losses: 3,
      tournaments: 8,
      manager: 'Alex Johnson',
      rank: 3,
      currentTournament: true,
      tournamentName: 'Valorant Championship'
    },
    {
      id: 2,
      name: 'Phoenix Squad',
      description: 'Rising from the ashes to claim victory',
      logo: '/assets/team-logos/phoenix-squad.png',
      logoUrl: '/assets/team-logos/phoenix-squad.png',
      memberCount: 4,
      maxMembers: 5,
      numberOfPlayers: 4,
      role: 'member',
      status: 'active',
      joinedDate: new Date('2024-02-20'),
      createdAt: new Date('2024-02-20'),
      wins: 8,
      losses: 5,
      tournaments: 4,
      manager: 'Sarah Chen',
      rank: 7
    },
    {
      id: 3,
      name: 'Digital Wolves',
      description: 'Pack hunters in the digital realm',
      logo: '/assets/team-logos/digital-wolves.png',
      memberCount: 3,
      maxMembers: 5,
      numberOfPlayers: 3,
      role: 'pending',
      status: 'active',
      joinedDate: new Date('2024-03-10'),
      createdAt: new Date('2024-03-10'),
      wins: 0,
      losses: 0,
      tournaments: 0,
      manager: 'Mike Wilson'
    }
  ]);

  isLoading = signal(false);

  teams = computed(() => this.allTeams());
  myTeams = computed(() => this.allTeams());
  
  activeTeams = computed(() => 
    this.teams().filter(team => team.status === 'active')
  );
  
  captainTeams = computed(() => 
    this.teams().filter(team => team.role === 'captain')
  );
  
  memberTeams = computed(() => 
    this.teams().filter(team => team.role === 'member')
  );
  
  pendingTeams = computed(() => 
    this.teams().filter(team => team.role === 'pending')
  );

  teamsAsLeader = computed(() => this.captainTeams());

  totalWins = computed(() => 
    this.teams().reduce((total, team) => total + team.wins, 0)
  );

  winRate = computed(() => {
    const teams = this.teams();
    const totalGames = teams.reduce((total, team) => total + team.wins + team.losses, 0);
    const totalWins = teams.reduce((total, team) => total + team.wins, 0);
    return totalGames > 0 ? Math.round((totalWins / totalGames) * 100) : 0;
  });

  selectedFilter = signal<'all' | 'captain' | 'member' | 'pending'>('all');

  filteredTeams = computed(() => {
    const filter = this.selectedFilter();
    switch (filter) {
      case 'captain':
        return this.captainTeams();
      case 'member':
        return this.memberTeams();
      case 'pending':
        return this.pendingTeams();
      default:
        return this.teams();
    }
  });

  getWinRate(team: Team): number {
    const totalGames = team.wins + team.losses;
    return totalGames > 0 ? (team.wins / totalGames) * 100 : 0;
  }

  getTeamInitials(name: string): string {
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 3);
  }

  getRoleBadgeClass(role: string): string {
    switch (role) {
      case 'captain':
        return 'role-captain';
      case 'member':
        return 'role-member';
      case 'pending':
        return 'role-pending';
      default:
        return '';
    }
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'active':
        return 'status-active';
      case 'inactive':
        return 'status-inactive';
      case 'disbanded':
        return 'status-disbanded';
      default:
        return '';
    }
  }

  setFilter(filter: 'all' | 'captain' | 'member' | 'pending'): void {
    this.selectedFilter.set(filter);
  }

  createTeam(): void {
    console.log('Creating new team');
  }

  findTeams(): void {
    console.log('Finding teams to join');
  }

  viewTeam(teamId: number): void {
    console.log(`Viewing team ${teamId}`);
  }

  viewInvitations(): void {
    console.log('Viewing team invitations');
  }

  leaveTeam(teamId: number): void {
    console.log(`Leaving team ${teamId}`);
  }

  manageTeam(teamId: number): void {
    console.log(`Managing team ${teamId}`);
  }

  acceptInvitation(teamId: number): void {
    console.log(`Accepting invitation to team ${teamId}`);
  }

  declineInvitation(teamId: number): void {
    console.log(`Declining invitation to team ${teamId}`);
  }
}
