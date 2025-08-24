import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { CommonModule, DatePipe } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.html',
  styleUrls: ['./create-tournament.css'],
  standalone: true,
  imports: [
    DashboardLayout,
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    DatePipe
  ],
})
export class CreateTournament {
  isSubmitting = false;
  isSubmitted = false;
  tournamentForm: FormGroup;

  constructor(
    private router: Router, 
    private fb: FormBuilder,
    private toast: ToastService
  ) {
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
    }, { validators: this.dateRangeValidator });
  }

  dateRangeValidator(group: FormGroup) {
    const startDate = group.get('startDate')?.value;
    const endDate = group.get('endDate')?.value;
    
    if (startDate && endDate) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      
      return start < end ? null : { dateRange: true };
    }
    
    return null;
  }

  onSubmit() {
    this.isSubmitted = true;
    
    if (this.tournamentForm.invalid) {
      this.toast.error('Please fill in all required fields correctly');
      return;
    }

    this.isSubmitting = true;

    // Simulate API call with delay
    setTimeout(() => {
      this.isSubmitting = false;
      this.toast.success('Tournament created successfully!');
      this.router.navigate(['/tournaments']);
    }, 1500);
  }
}
