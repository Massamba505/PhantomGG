import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { Team, Tournament } from '@/app/shared/models/tournament';
import { TeamCard } from '@/app/shared/components/team-card/team-card';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-teams-section',
  template: `
    <div class="mb-6">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-semibold">Registered Teams</h2>
        <div class="text-sm">
          {{ teams().length || 0 }} of {{ tournament()?.maxTeams || 0 }} teams
        </div>
      </div>

      @if (loading()) {
        <!-- Loading State -->
        <div class="space-y-4">
          <div class="text-center py-8">
            <div class="inline-flex items-center justify-center w-12 h-12 rounded-full bg-muted mb-4">
              <lucide-angular 
                [img]="icons.Loader2" 
                size="20" 
                class="animate-spin text-muted-foreground"
              ></lucide-angular>
            </div>
            <p class="text-muted-foreground">Loading teams...</p>
          </div>
        </div>
      } @else {
        <!-- Teams Grid Display -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          @for (team of teams(); track team.id) {
            <div>
              <app-team-card
                [team]="team"
                [showActions]="true"
                (edit)="onEditTeam($event)"
                (delete)="onDeleteTeam(team.id)"
              ></app-team-card>
            </div>
          }
          
          @if (tournament() && teams().length < tournament()!.maxTeams && teams().length !== 0) {
            <div
              class="border-2 border-dashed border-border rounded-lg flex flex-col items-center justify-center p-6 h-full cursor-pointer transition-colors hover:border-primary hover:bg-muted/30"
              (click)="onAddTeam()"
            >
              <div class="w-12 h-12 rounded-full bg-muted flex items-center justify-center mb-4">
                <lucide-angular 
                  [img]="icons.UserPlus" 
                  size="20"
                  class="text-muted-foreground"
                ></lucide-angular>
              </div>
              <h3 class="font-medium mb-1">Add New Team</h3>
              <p class="text-sm text-center text-muted-foreground">
                Click to register a team for this tournament
              </p>
            </div>
          }
        </div>

        <!-- Empty state for no teams -->
        @if (teams().length === 0) {
          <div class="border-2 border-dashed border-border rounded-lg flex flex-col items-center justify-center p-12 text-center">
            <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-muted flex items-center justify-center">
              <lucide-angular 
                [img]="icons.Users" 
                size="24"
                class="text-muted-foreground"
              ></lucide-angular>
            </div>
            <h3 class="text-lg font-semibold mb-2">No teams registered yet</h3>
            <p class="text-muted-foreground mb-6 max-w-sm">
              Add teams to get started with your tournament. You can register up to {{ tournament()?.maxTeams }} teams.
            </p>
            <button 
              (click)="onAddTeam()" 
              class="inline-flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
            >
              <lucide-angular [img]="icons.UserPlus" size="16"></lucide-angular>
              Add First Team
            </button>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .animate-spin {
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      from {
        transform: rotate(0deg);
      }
      to {
        transform: rotate(360deg);
      }
    }
  `],
  standalone: true,
  imports: [CommonModule, LucideAngularModule, TeamCard]
})
export class TeamsSection {
  teams = input.required<Team[]>();
  tournament = input.required<Tournament | null>();
  loading = input<boolean>(false);

  addTeam = output<void>();
  editTeam = output<Team>();
  deleteTeam = output<string>();

  readonly icons = LucideIcons;

  onAddTeam() {
    this.addTeam.emit();
  }

  onEditTeam(team: Team) {
    this.editTeam.emit(team);
  }

  onDeleteTeam(teamId: string) {
    this.deleteTeam.emit(teamId);
  }
}
