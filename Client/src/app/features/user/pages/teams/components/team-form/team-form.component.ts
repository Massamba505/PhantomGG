import { Component, Input, Output, EventEmitter, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateTeam, UpdateTeam, Team } from '@/app/api/models/team.models';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';

@Component({
  selector: 'app-team-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-form.component.html',
  styleUrl: './team-form.component.css'
})
export class TeamFormComponent implements OnInit {
  @Input() team: Team | null = null;
  @Input() set loadingState(value: boolean) {
    this.loading.set(value);
  }
  @Output() formSubmit = new EventEmitter<CreateTeam>();
  @Output() formUpdate = new EventEmitter<UpdateTeam>();
  @Output() formCancel = new EventEmitter<void>();
  
  private readonly teamService = inject(TeamService);
  private readonly toastService = inject(ToastService);
  private fb = inject(FormBuilder);
  
  loading = signal(false);
  isEdit = signal(false);
  uploadingLogo = signal(false);
  logoPreview = signal<string | null>(null);
  currentLogoUrl = signal<string | null>(null);

  teamForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    shortName: ['', [Validators.maxLength(5)]],
    logoUrl: [''],
    managerName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]]
  });

  ngOnInit() {
    if (this.team) {
      this.isEdit.set(true);
      this.currentLogoUrl.set(this.team.logoUrl || null);
      this.teamForm.patchValue({
        name: this.team.name,
        shortName: this.team.shortName || '',
        logoUrl: this.team.logoUrl || '',
        managerName: '' // This would come from user profile in a real scenario
      });
    }
  }

  onSubmit() {
    if (this.teamForm.valid) {
      const formData = this.teamForm.value;

      if (this.isEdit()) {
        const updateData: UpdateTeam = {
          name: formData.name!,
          shortName: formData.shortName || undefined,
          logoUrl: formData.logoUrl || undefined
        };
        this.formUpdate.emit(updateData);
      } else {
        const createData: CreateTeam = {
          name: formData.name!,
          shortName: formData.shortName || undefined,
          managerName: formData.managerName!,
          logoUrl: undefined, // File upload will be handled separately
          teamPhotoUrl: undefined,
          tournamentId: '' // Not needed for team creation
        };
        this.formSubmit.emit(createData);
      }
    } else {
      this.teamForm.markAllAsTouched();
    }
  }

  onCancel() {
    this.formCancel.emit();
  }

  onLogoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Create preview
      const reader = new FileReader();
      reader.onload = (e) => {
        this.logoPreview.set(e.target?.result as string);
      };
      reader.readAsDataURL(file);

      // In a real implementation, you would upload the file here
      // For now, we'll just update the form control with a placeholder
      this.teamForm.patchValue({ logoUrl: 'uploaded-logo-url' });
    }
  }

  removeLogo() {
    this.logoPreview.set(null);
    this.teamForm.patchValue({ logoUrl: '' });
  }

  // Form validation helpers
  getFieldError(fieldName: string): string | null {
    const field = this.teamForm.get(fieldName);
    if (field && field.invalid && field.touched) {
      const errors = field.errors;
      if (errors?.['required']) {
        return `${this.getFieldLabel(fieldName)} is required`;
      }
      if (errors?.['minlength']) {
        return `${this.getFieldLabel(fieldName)} must be at least ${errors['minlength'].requiredLength} characters`;
      }
      if (errors?.['maxlength']) {
        return `${this.getFieldLabel(fieldName)} cannot exceed ${errors['maxlength'].requiredLength} characters`;
      }
    }
    return null;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.teamForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      name: 'Team name',
      shortName: 'Short name',
      managerName: 'Manager name'
    };
    return labels[fieldName] || fieldName;
  }
}