import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ArrowLeft, Plus, Trophy, Ghost, Zap, Target } from 'lucide-angular';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.component.html',
  styleUrls: ['./create-tournament.component.css'],
  standalone: true,
  imports: [DashboardLayout],
})
export class CreateTournamentComponent {
  isSubmitting = false;
  tournamentForm: FormGroup;

  constructor(private router: Router, private fb: FormBuilder) {
    this.tournamentForm = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      maxTeams: [
        8,
        [Validators.required, Validators.min(4), Validators.max(32)],
      ],
      location: ['', Validators.required],
      entryFee: [0, [Validators.required, Validators.min(0)]],
      prizePool: [0, [Validators.required, Validators.min(0)]],
      contactEmail: ['', [Validators.required, Validators.email]],
    });
  }

  onSubmit() {
    if (this.tournamentForm.invalid) return;

    this.isSubmitting = true;

    setTimeout(() => {
      this.isSubmitting = false;
      this.router.navigate(['/tournaments']);
    }, 1500);
  }
}
