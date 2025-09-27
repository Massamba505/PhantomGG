import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { CreateTeam, UpdateTeam, Team } from '@/app/api/models/team.models';

@Component({
  selector: 'app-team-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './team-form.component.html',
  styleUrl: './team-form.component.css'
})
export class TeamFormComponent implements OnInit, OnChanges {
  @Input() team: Team | null = null;
  @Input() tournamentId: string | null = null;
  @Output() formSubmit = new EventEmitter<CreateTeam | UpdateTeam>();
  @Output() formCancel = new EventEmitter<void>();
  
  private fb = inject(FormBuilder);
  
  teamForm!: FormGroup;
  submitted = signal(false);
  logoPreview = signal<string | null>(null);

  isEditMode = computed(() => this.team !== null);

  ngOnInit() {
    console.log('TeamForm ngOnInit - team:', this.team);
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    console.log('TeamForm ngOnChanges - changes:', changes);
    if (changes['team'] && this.teamForm) {
      console.log('Team changed, reinitializing form with:', changes['team'].currentValue);
      this.initializeForm();
    }
  }

  private initializeForm() {
    const team = this.team;
    console.log('Initializing form with team:', team);
    
    this.teamForm = this.fb.group({
      name: [
        team?.name || '',
        [Validators.required, Validators.minLength(2), Validators.maxLength(200)],
      ],
      shortName: [
        team?.shortName || '',
        [Validators.maxLength(10)],
      ],
      logo: [null], // Optional for both create and edit
    });

    // Set logo preview if team has logoUrl
    if (team?.logoUrl) {
      this.logoPreview.set(team.logoUrl);
    } else {
      this.logoPreview.set(null);
    }

    console.log('Form initialized with values:', this.teamForm.value);
  }

  // Handle file select
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

  onSubmit() {
    this.submitted.set(true);
    if (this.teamForm.invalid) {
      return;
    }

    const formValue = this.teamForm.value;

    if (this.isEditMode()) {
      // For edit mode
      const updateData: UpdateTeam = {
        name: formValue.name,
        shortName: formValue.shortName || undefined,
        logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
        teamPhotoUrl: undefined
      };

      this.formSubmit.emit(updateData);
    } else {
      // For create mode
      const createData: CreateTeam = {
        name: formValue.name,
        shortName: formValue.shortName || undefined,
        logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
        teamPhotoUrl: undefined
      };

      this.formSubmit.emit(createData);
    }
  }

  onCancel() {
    this.formCancel.emit();
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.teamForm.get(fieldName);
    return !!(field && field.invalid && this.submitted());
  }
}