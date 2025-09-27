import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Team, CreateTeam, UpdateTeam } from '@/app/api/models/team.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-team-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    LucideAngularModule
  ],
  templateUrl: './team-form.html',
  styleUrl: './team-form.css'
})
export class TeamForm implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  
  readonly icons = LucideIcons;
  
  teamForm: FormGroup;
  isLoading = signal(false);
  isSubmitting = signal(false);
  isEditMode = signal(false);
  teamId = signal<string | null>(null);
  team = signal<Team | null>(null);
  logoPreview = signal<string | null>(null);

  constructor() {
    this.teamForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      shortName: ['', [Validators.maxLength(10)]],
      logo: [null]
    });
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.teamId.set(id);
      this.loadTeam(id);
    }
  }

  loadTeam(id: string) {
    this.isLoading.set(true);
    
    this.teamService.getTeam(id).subscribe({
      next: (team) => {
        this.team.set(team);
        this.teamForm.patchValue({
          name: team.name,
          shortName: team.shortName,
          logo: null // Don't set existing logo file
        });
        // Set logo preview if team has logoUrl
        if (team.logoUrl) {
          this.logoPreview.set(team.logoUrl);
        }
      },
      error: (error) => {
        console.error('Failed to load team:', error);
        this.toastService.error('Failed to load team');
        this.goBack();
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  onSubmit() {
    if (this.teamForm.invalid) {
      this.teamForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    if (this.isEditMode()) {
      this.updateTeam();
    } else {
      this.createTeam();
    }
  }

  private createTeam() {
    const formValue = this.teamForm.value;
    const createDto: CreateTeam = {
      name: formValue.name,
      shortName: formValue.shortName || undefined,
      logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
      teamPhotoUrl: undefined
    };

    this.teamService.createTeam(createDto).subscribe({
      next: (team) => {
        this.toastService.success('Team created successfully');
        this.router.navigate(['/user/teams']);
      },
      error: (error) => {
        console.error('Failed to create team:', error);
        this.toastService.error('Failed to create team');
      },
      complete: () => {
        this.isSubmitting.set(false);
      }
    });
  }

  private updateTeam() {
    const formValue = this.teamForm.value;
    const updateDto: UpdateTeam = {
      name: formValue.name,
      shortName: formValue.shortName || undefined,
      logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
      teamPhotoUrl: undefined
    };

    this.teamService.updateTeam(this.teamId()!, updateDto).subscribe({
      next: (team) => {
        this.toastService.success('Team updated successfully');
        this.router.navigate(['/user/teams']);
      },
      error: (error) => {
        console.error('Failed to update team:', error);
        this.toastService.error('Failed to update team');
      },
      complete: () => {
        this.isSubmitting.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/user/teams']);
  }

  // Handle file upload
  onLogoChange(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      const file = target.files[0];
      this.teamForm.patchValue({ logo: file });
      this.teamForm.get('logo')?.updateValueAndValidity();

      // Show preview
      const reader = new FileReader();
      reader.onload = () => this.logoPreview.set(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  // Form field helper methods
  getFieldError(fieldName: string): string | null {
    const field = this.teamForm.get(fieldName);
    if (field && field.invalid && field.touched) {
      const errors = field.errors;
      if (errors?.['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
      if (errors?.['minlength']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be at least ${errors['minlength'].requiredLength} characters`;
      }
      if (errors?.['maxlength']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} cannot exceed ${errors['maxlength'].requiredLength} characters`;
      }
    }
    return null;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.teamForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}