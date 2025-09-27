import { Component, input, output, OnInit, OnChanges, SimpleChanges, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { CreateTournament, Tournament, UpdateTournament } from '@/app/api/models';

@Component({
  selector: 'app-tournament-form',
  templateUrl: './tournament-form.html',
  styleUrls: ['./tournament-form.css'],
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
})
export class TournamentForm implements OnInit, OnChanges {
  tournament = input<Tournament | null>(null);
  mode = input<'create' | 'edit'>('create');
  tournamentSaved = output<CreateTournament>();
  tournamentUpdated = output<UpdateTournament>();
  cancelled = output<void>();

  private fb = inject(FormBuilder);

  tournamentForm!: FormGroup;
  isSubmitted = signal(false);
  isSubmitting = signal(false);
  bannerPreview = signal<string | null>(null);
  logoPreview = signal<string | null>(null);
  isDragging = signal(false);
  readonly icons = LucideIcons;

  isEditMode = computed(() => this.mode() === 'edit');
  
  formTitle = computed(() => 
    this.isEditMode() ? 'Edit Tournament' : 'Create Tournament'
  );

  submitButtonText = computed(() => {
    if (this.isSubmitting()) {
      return this.isEditMode() ? 'Updating...' : 'Creating...';
    }
    return this.isEditMode() ? 'Update Tournament' : 'Create Tournament';
  });

  ngOnInit() {
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['tournament'] && this.tournamentForm) {
      this.initializeForm();
    }
  }

  private initializeForm() {
    const tournament = this.tournament();
    this.tournamentForm = this.fb.group({
      name: [tournament?.name || '', [Validators.required, Validators.minLength(3)]],
      description: [tournament?.description || '', [Validators.required, Validators.minLength(10)]],
      location: [tournament?.location || '', [Validators.required]],
      registrationDeadline: [tournament?.registrationDeadline || '', [Validators.required]],
      startDate: [tournament?.startDate || '', [Validators.required]],
      endDate: [tournament?.endDate || '', [Validators.required]],
      maxTeams: [tournament?.maxTeams || 16, [Validators.required]],
      contactEmail: [tournament?.organizer?.email || '', [Validators.email]],
      banner: [null], // file control for banner - optional
      logo: [null], // file control for logo - optional
    }, { validators: [this.dateRangeValidator, this.deadlineValidator] });

    // Set banner preview if tournament has bannerUrl
    if (tournament?.bannerUrl) {
      this.bannerPreview.set(tournament.bannerUrl);
    } else {
      this.bannerPreview.set(null);
    }

    // Set logo preview if tournament has logoUrl
    if (tournament?.logoUrl) {
      this.logoPreview.set(tournament.logoUrl);
    } else {
      this.logoPreview.set(null);
    }
  }

  private dateRangeValidator = (form: FormGroup) => {
    const startDate = form.get('startDate')?.value;
    const endDate = form.get('endDate')?.value;
    
    if (startDate && endDate && new Date(startDate) >= new Date(endDate)) {
      return { dateRange: true };
    }
    return null;
  };

  private deadlineValidator = (form: FormGroup) => {
    const deadline = form.get('registrationDeadline')?.value;
    const startDate = form.get('startDate')?.value;
    
    if (deadline && startDate && new Date(deadline) >= new Date(startDate)) {
      return { deadlineInvalid: true };
    }
    return null;
  };

  onBannerChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.handleBannerFileSelect(input.files[0]);
    }
  }

  onLogoChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.handleLogoFileSelect(input.files[0]);
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging.set(true);
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging.set(false);
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging.set(false);
    
    if (event.dataTransfer?.files && event.dataTransfer.files[0]) {
      this.handleBannerFileSelect(event.dataTransfer.files[0]);
    }
  }

  private handleBannerFileSelect(file: File) {
    if (!file.type.startsWith('image/')) {
      return;
    }

    this.tournamentForm.patchValue({ banner: file });
    this.tournamentForm.get('banner')?.updateValueAndValidity();

    const reader = new FileReader();
    reader.onload = () => {
      this.bannerPreview.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  private handleLogoFileSelect(file: File) {
    if (!file.type.startsWith('image/')) {
      return;
    }

    this.tournamentForm.patchValue({ logo: file });
    this.tournamentForm.get('logo')?.updateValueAndValidity();

    const reader = new FileReader();
    reader.onload = () => {
      this.logoPreview.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  onSubmit() {
    this.isSubmitted.set(true);
    
    if (this.tournamentForm.invalid) {
      return;
    }

    this.isSubmitting.set(true);

    const formValue = this.tournamentForm.value;

    if (this.isEditMode()) {
      const updateData: UpdateTournament = {
        name: formValue.name,
        description: formValue.description,
        location: formValue.location,
        registrationDeadline: formValue.registrationDeadline,
        startDate: formValue.startDate,
        endDate: formValue.endDate,
        maxTeams: parseInt(formValue.maxTeams, 10),
        contactEmail: formValue.contactEmail,
        bannerUrl: formValue.banner instanceof File ? formValue.banner : undefined,
        logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
        minTeams: 2,
        isPublic: true
      };
      this.tournamentUpdated.emit(updateData);
    } else {
      const createData: CreateTournament = {
        name: formValue.name,
        description: formValue.description,
        location: formValue.location,
        registrationDeadline: formValue.registrationDeadline,
        startDate: formValue.startDate,
        endDate: formValue.endDate,
        maxTeams: parseInt(formValue.maxTeams, 10),
        contactEmail: formValue.contactEmail,
        bannerUrl: formValue.banner instanceof File ? formValue.banner : undefined,
        logoUrl: formValue.logo instanceof File ? formValue.logo : undefined,
        minTeams: 2,
        isPublic: true
      };
      this.tournamentSaved.emit(createData);
    }
    
    // Reset form state after emission
    setTimeout(() => {
      this.isSubmitting.set(false);
    }, 100);
  }

  onCancel() {
    this.cancelled.emit();
  }

  resetForm() {
    this.isSubmitting.set(false);
    this.isSubmitted.set(false);
    this.bannerPreview.set(null);
    this.logoPreview.set(null);
    this.tournamentForm.reset({
      maxTeams: 16
    });
  }
}
