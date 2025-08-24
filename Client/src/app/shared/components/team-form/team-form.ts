import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Team, TeamFormData } from '../../models/tournament';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-team-form',
  templateUrl: './team-form.html',
  styleUrls: ['./team-form.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class TeamForm implements OnInit {
  @Input() team: Team | null = null;
  @Output() save = new EventEmitter<TeamFormData>();
  @Output() cancel = new EventEmitter<void>();

  formData: TeamFormData = {
    name: '',
    city: '',
    coach: '',
    players: 11,
  };

  submitted = false;

  constructor(private toastService: ToastService) {}

  ngOnInit() {
    if (this.team) {
      this.formData = {
        name: this.team.name,
        city: this.team.city,
        coach: this.team.coach,
        players: this.team.players,
      };
    }
  }

  onSubmit() {
    this.submitted = true;
    
    // Only emit if form is valid - the HTML handles the validation display
    if (this.formData.name && this.formData.city && this.formData.coach && this.formData.players) {
      this.save.emit(this.formData);
      this.toastService.success(this.team ? 'Team updated successfully' : 'Team added successfully');
    }
  }

  onCancel() {
    this.cancel.emit();
  }
}
