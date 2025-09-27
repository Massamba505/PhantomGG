import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { CreateTournament, UpdateTournament, Tournament } from '@/app/api/models/tournament.models';

@Component({
  selector: 'app-tournament-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tournament-form.component.html',
  styleUrl: './tournament-form.component.css'
})
export class TournamentFormComponent implements OnInit, OnChanges {
  @Input() tournament: Tournament | null = null;
  @Input() set loadingState(value: boolean) {
    this.loading.set(value);
  }
  @Output() formSubmit = new EventEmitter<CreateTournament>();
  @Output() formUpdate = new EventEmitter<UpdateTournament>();
  @Output() formCancel = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  
  tournamentForm!: FormGroup;
  loading = signal(false);
  submitted = signal(false);
  bannerPreview = signal<string | null>(null);
  logoPreview = signal<string | null>(null);

  isEditMode = computed(() => this.tournament !== null);
  
  get todayDateTime(): string {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  }

  ngOnInit() {
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['tournament'] && this.tournamentForm) {
      this.initializeForm();
    }
  }

  private initializeForm() {
    const tournament = this.tournament;
    
    this.tournamentForm = this.fb.group({
      name: [
        tournament?.name || '',
        [Validators.required, Validators.minLength(3), Validators.maxLength(200)],
      ],
      description: [
        tournament?.description || '',
        [Validators.required, Validators.minLength(10), Validators.maxLength(2000)],
      ],
      location: [
        tournament?.location || '',
        [Validators.maxLength(200)],
      ],
      registrationStartDate: [
        tournament?.registrationStartDate ? 
          new Date(tournament.registrationStartDate).toISOString().slice(0, 16) : '',
        [Validators.required],
      ],
      registrationDeadline: [
        tournament?.registrationDeadline ? 
          new Date(tournament.registrationDeadline).toISOString().slice(0, 16) : '',
        [Validators.required],
      ],
      startDate: [
        tournament?.startDate ? 
          new Date(tournament.startDate).toISOString().slice(0, 16) : '',
        [Validators.required],
      ],
      endDate: [
        tournament?.endDate ? 
          new Date(tournament.endDate).toISOString().slice(0, 16) : '',
        [Validators.required],
      ],
      minTeams: [tournament?.minTeams || 2, [Validators.required, Validators.min(2), Validators.max(64)]],
      maxTeams: [tournament?.maxTeams || 16, [Validators.required, Validators.min(4), Validators.max(128)]],
      contactEmail: [tournament?.organizer?.email || '', [Validators.email]],
      isPublic: [tournament?.isPublic ?? true],
      banner: [null], // file control for banner - optional
      logo: [null], // file control for logo - optional
    });

    // Set previews if tournament has existing images
    if (tournament?.bannerUrl) {
      this.bannerPreview.set(tournament.bannerUrl);
    } else {
      this.bannerPreview.set(null);
    }

    if (tournament?.logoUrl) {
      this.logoPreview.set(tournament.logoUrl);
    } else {
      this.logoPreview.set(null);
    }
  }

  // Handle banner file select
  onBannerChange(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      const file = target.files[0];
      this.tournamentForm.patchValue({ banner: file });
      this.tournamentForm.get('banner')?.updateValueAndValidity();

      // Show preview
      const reader = new FileReader();
      reader.onload = () => this.bannerPreview.set(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  // Handle logo file select
  onLogoChange(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      const file = target.files[0];
      this.tournamentForm.patchValue({ logo: file });
      this.tournamentForm.get('logo')?.updateValueAndValidity();

      // Show preview
      const reader = new FileReader();
      reader.onload = () => this.logoPreview.set(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    this.submitted.set(true);
    if (this.tournamentForm.invalid) {
      return;
    }

    const formValue = this.tournamentForm.value;

    if (this.isEditMode()) {
      // For edit mode
      const updateData: UpdateTournament = {
        name: formValue.name,
        description: formValue.description,
        location: formValue.location || undefined,
        registrationStartDate: formValue.registrationStartDate || undefined,
        registrationDeadline: formValue.registrationDeadline || undefined,
        startDate: formValue.startDate || undefined,
        endDate: formValue.endDate || undefined,
        minTeams: formValue.minTeams || undefined,
        maxTeams: formValue.maxTeams || undefined,
        contactEmail: formValue.contactEmail || undefined,
        isPublic: formValue.isPublic ?? undefined,
        bannerUrl: formValue.banner instanceof File ? formValue.banner : undefined,
        logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
      };

      this.formUpdate.emit(updateData);
    } else {
      // For create mode
      const createData: CreateTournament = {
        name: formValue.name,
        description: formValue.description,
        location: formValue.location || undefined,
        registrationStartDate: formValue.registrationStartDate || undefined,
        registrationDeadline: formValue.registrationDeadline || undefined,
        startDate: formValue.startDate,
        endDate: formValue.endDate,
        minTeams: formValue.minTeams,
        maxTeams: formValue.maxTeams,
        contactEmail: formValue.contactEmail || undefined,
        isPublic: formValue.isPublic ?? true,
        bannerUrl: formValue.banner instanceof File ? formValue.banner : undefined,
        logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
      };

      this.formSubmit.emit(createData);
    }
  }

  onCancel() {
    this.formCancel.emit();
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.tournamentForm.get(fieldName);
    return !!(field && field.invalid && this.submitted());
  }
}
