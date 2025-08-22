import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Team, TeamFormData } from '../../models/tournament';

@Component({
  selector: 'app-team-form',
  templateUrl: './team-form.component.html',
  styleUrls: ['./team-form.component.css'],
  standalone: true,
})
export class TeamFormComponent {
  @Input() team: Team | null = null;
  @Output() save = new EventEmitter<TeamFormData>();
  @Output() cancel = new EventEmitter<void>();

  formData: TeamFormData = this.team
    ? {
        name: this.team.name,
        city: this.team.city,
        coach: this.team.coach,
        players: this.team.players,
      }
    : {
        name: '',
        city: '',
        coach: '',
        players: 11,
      };

  onSubmit() {
    this.save.emit(this.formData);
  }

  onCancel() {
    this.cancel.emit();
  }
}
