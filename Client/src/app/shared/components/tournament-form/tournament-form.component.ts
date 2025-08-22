import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Tournament, TournamentFormData } from '../../models/tournament';

@Component({
  selector: 'app-tournament-form',
  templateUrl: './tournament-form.component.html',
  styleUrls: ['./tournament-form.component.css'],
  standalone: true,
})
export class TournamentFormComponent {
  @Input() tournament: Tournament | null = null;
  @Output() save = new EventEmitter<TournamentFormData>();
  @Output() cancel = new EventEmitter<void>();

  formData: TournamentFormData = this.tournament
    ? {
        name: this.tournament.name,
        description: this.tournament.description,
        startDate: this.tournament.startDate,
        endDate: this.tournament.endDate,
        maxTeams: this.tournament.maxTeams,
      }
    : {
        name: '',
        description: '',
        startDate: '',
        endDate: '',
        maxTeams: 8,
      };

  onSubmit() {
    this.save.emit(this.formData);
  }

  onCancel() {
    this.cancel.emit();
  }
}
