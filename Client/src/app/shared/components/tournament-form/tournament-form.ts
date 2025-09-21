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
  standalone: true,
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
    }, { validators: [this.dateRangeValidator, this.deadlineValidator] });

    if (tournament?.bannerUrl) {
      this.bannerPreview.set(tournament.bannerUrl);
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
      this.handleFileSelect(input.files[0]);
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
      this.handleFileSelect(event.dataTransfer.files[0]);
    }
  }

  private handleFileSelect(file: File) {
    if (!file.type.startsWith('image/')) {
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      this.bannerPreview.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  onSubmit() {
    this.isSubmitted.set(true);
    
    if (this.tournamentForm.invalid) {
      return;
    }

    this.isSubmitting.set(true);

    if (this.isEditMode()) {
      const updateData: UpdateTournament = {
        name: this.tournamentForm.value.name,
        description: this.tournamentForm.value.description,
        location: this.tournamentForm.value.location,
        registrationDeadline: this.tournamentForm.value.registrationDeadline,
        startDate: this.tournamentForm.value.startDate,
        registrationStartDate: this.tournamentForm.value.endDate,
        maxTeams: parseInt(this.tournamentForm.value.maxTeams, 10),
        contactEmail: this.tournamentForm.value.contactEmail,
        bannerUrl: this.bannerPreview() || undefined,
      };
      this.tournamentUpdated.emit(updateData);
    } else {
      const createData: CreateTournament = {
        name: this.tournamentForm.value.name,
        description: this.tournamentForm.value.description,
        location: this.tournamentForm.value.location,
        registrationDeadline: this.tournamentForm.value.registrationDeadline,
        startDate: this.tournamentForm.value.startDate,
        endDate: this.tournamentForm.value.endDate,
        registrationStartDate: this.tournamentForm.value.endDate,
        maxTeams: parseInt(this.tournamentForm.value.maxTeams, 10),
        contactEmail: this.tournamentForm.value.contactEmail,
        bannerUrl: this.bannerPreview() || undefined,
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
    this.tournamentForm.reset({
      maxTeams: 16,
      entryFee: 0,
      prize: 0
    });
  }
}
