import { NgClass } from '@angular/common';
import { Component, input, output, OnInit, OnChanges, SimpleChanges, signal, computed, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CreateTeam, Team, UpdateTeam } from '@/app/api/models';

@Component({
  selector: 'app-team-form',
  templateUrl: './team-form.html',
  imports: [NgClass, ReactiveFormsModule],
})
export class TeamForm implements OnInit, OnChanges {
  team = input<Team | null>(null);
  tournamentId = input<string | null>(null);
  save = output<CreateTeam | UpdateTeam>();
  cancel = output<void>();

  private fb = inject(FormBuilder);

  teamForm!: FormGroup;
  submitted = signal(false);
  logoPreview = signal<string | null>(null);

  isEditMode = computed(() => this.team() !== null);

  ngOnInit() {
    console.log('TeamForm ngOnInit - team:', this.team());
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
    const team = this.team();
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
      logo: [null], // file control - optional
    });

    // Set logo preview if team has logoUrl
    if (team?.logoUrl) {
      this.logoPreview.set(team.logoUrl);
    } else {
      this.logoPreview.set(null);
    }

    console.log('Form initialized with values:', this.teamForm.value);
  }

  // Handle file select or drop
  onLogoChange(event: Event | DragEvent) {
    let file: File | null = null;

    if (event instanceof DragEvent && event.dataTransfer?.files.length) {
      file = event.dataTransfer.files[0];
    } else if (
      event.target instanceof HTMLInputElement &&
      event.target.files?.length
    ) {
      file = event.target.files[0];
    }

    if (file) {
      this.teamForm.patchValue({ logo: file });
      this.teamForm.get('logo')?.updateValueAndValidity();

      const reader = new FileReader();
      reader.onload = () => this.logoPreview.set(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  allowDrop(event: DragEvent) {
    event.preventDefault();
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

      this.save.emit(updateData);
    } else {
      // For create mode
      const createData: CreateTeam = {
        name: formValue.name,
        shortName: formValue.shortName || undefined,
        logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
        teamPhotoUrl: undefined
      };

      this.save.emit(createData);
    }
  }

  onCancel() {
    this.cancel.emit();
  }
}
