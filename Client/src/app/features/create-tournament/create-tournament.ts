import { Component } from '@angular/core';
import { Router } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { ToastService } from '@/app/shared/services/toast.service';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.html',
  styleUrls: ['./create-tournament.css'],
  standalone: true,
  imports: [DashboardLayout, CommonModule, ReactiveFormsModule],
})
export class CreateTournament {
  isSubmitting = false;
  isSubmitted = false;
  bannerPreview: string | null = null;

  tournamentForm: FormGroup;

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private toast: ToastService
  ) {
    this.tournamentForm = this.fb.group(
      {
        name: ['', [Validators.required, Validators.minLength(3)]],
        description: ['', [Validators.required, Validators.minLength(10)]],
        location: ['', Validators.required],
        registrationDeadline: ['', Validators.required],
        startDate: ['', Validators.required],
        endDate: ['', Validators.required],
        maxTeams: [
          8,
          [Validators.required, Validators.min(4), Validators.max(64)],
        ],
        entryFee: [0, [Validators.required, Validators.min(0)]],
        prizePool: [0, [Validators.required, Validators.min(0)]],
        contactEmail: [
          '',
          [Validators.required, Validators.email, strictEmailValidator],
        ],
        banner: [null],
      },
      {
        validators: [
          this.dateRangeValidator,
          this.deadlineBeforeStartValidator,
        ],
      }
    );
  }

  // Ensure startDate < endDate
  dateRangeValidator(group: FormGroup) {
    const startDate = group.get('startDate')?.value;
    const endDate = group.get('endDate')?.value;

    if (startDate && endDate) {
      return new Date(startDate) < new Date(endDate)
        ? null
        : { dateRange: true };
    }
    return null;
  }

  // Ensure registrationDeadline < startDate
  deadlineBeforeStartValidator(group: FormGroup) {
    const deadline = group.get('registrationDeadline')?.value;
    const startDate = group.get('startDate')?.value;

    if (deadline && startDate) {
      return new Date(deadline) < new Date(startDate)
        ? null
        : { deadlineInvalid: true };
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

    // Simulate API call
    setTimeout(() => {
      this.isSubmitting = false;
      this.toast.success('Tournament created successfully!');
      this.router.navigate(['/tournaments']);
    }, 1500);
  }

  isDragging = false;

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;

    const file = event.dataTransfer?.files[0];
    if (file) {
      this.handleBannerFile(file);
    }
  }

  onBannerChange(event: Event) {
    const file = (event.target as HTMLInputElement)?.files?.[0];
    if (file) {
      this.handleBannerFile(file);
    }
  }

  private handleBannerFile(file: File) {
    this.tournamentForm.patchValue({ banner: file });
    this.tournamentForm.get('banner')?.updateValueAndValidity();

    const reader = new FileReader();
    reader.onload = () => (this.bannerPreview = reader.result as string);
    reader.readAsDataURL(file);
  }
}
