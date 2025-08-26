import { Component, EventEmitter, Input, OnInit, Output, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Tournament, CreateTournamentRequest, UpdateTournamentRequest } from '@/app/shared/models/tournament';

@Component({
  selector: 'app-tournament-form',
  templateUrl: './tournament-form.html',
  styleUrls: ['./tournament-form.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
})
export class TournamentForm implements OnInit, OnChanges {
  @Input() tournament: Tournament | null = null;
  @Input() mode: 'create' | 'edit' = 'create';
  @Output() tournamentSaved = new EventEmitter<CreateTournamentRequest | UpdateTournamentRequest>();
  @Output() cancelled = new EventEmitter<void>();

  private fb = inject(FormBuilder);

  tournamentForm!: FormGroup;
  isSubmitted = false;
  isSubmitting = false;
  bannerPreview: string | null = null;
  isDragging = false;
  readonly icons = LucideIcons;

  ngOnInit() {
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['tournament'] && this.tournamentForm) {
      this.initializeForm();
    }
  }

  private initializeForm() {
    this.tournamentForm = this.fb.group({
      name: [this.tournament?.name || '', [Validators.required, Validators.minLength(3)]],
      description: [this.tournament?.description || '', [Validators.required, Validators.minLength(10)]],
      location: [this.tournament?.location || '', [Validators.required]],
      registrationDeadline: [this.tournament?.registrationDeadline || '', [Validators.required]],
      startDate: [this.tournament?.startDate || '', [Validators.required]],
      endDate: [this.tournament?.endDate || '', [Validators.required]],
      maxTeams: [this.tournament?.maxTeams || 16, [Validators.required]],
      entryFee: [this.tournament?.entryFee || 0],
      prize: [this.tournament?.prize || 0],
      contactEmail: [this.tournament?.contactEmail || '', [Validators.required, Validators.email]],
    }, { validators: [this.dateRangeValidator, this.deadlineValidator] });

    if (this.tournament?.bannerUrl) {
      this.bannerPreview = this.tournament.bannerUrl;
    }
  }

  get isEditMode(): boolean {
    return this.mode === 'edit';
  }

  get formTitle(): string {
    return this.isEditMode ? 'Edit Tournament' : 'Create Tournament';
  }

  get submitButtonText(): string {
    if (this.isSubmitting) {
      return this.isEditMode ? 'Updating...' : 'Creating...';
    }
    return this.isEditMode ? 'Update Tournament' : 'Create Tournament';
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
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
    
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
      this.bannerPreview = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  onSubmit() {
    this.isSubmitted = true;
    
    if (this.tournamentForm.invalid) {
      return;
    }

    this.isSubmitting = true;

    const tournamentData: CreateTournamentRequest | UpdateTournamentRequest = {
      name: this.tournamentForm.value.name,
      description: this.tournamentForm.value.description,
      location: this.tournamentForm.value.location,
      registrationDeadline: this.tournamentForm.value.registrationDeadline,
      startDate: this.tournamentForm.value.startDate,
      endDate: this.tournamentForm.value.endDate,
      maxTeams: parseInt(this.tournamentForm.value.maxTeams, 10),
      entryFee: this.tournamentForm.value.entryFee || 0,
      prize: this.tournamentForm.value.prize || 0,
      contactEmail: this.tournamentForm.value.contactEmail,
      bannerUrl: this.bannerPreview || undefined,
    };

    this.tournamentSaved.emit(tournamentData);
    
    // Reset form state after emission
    setTimeout(() => {
      this.isSubmitting = false;
    }, 100);
  }

  onCancel() {
    this.cancelled.emit();
  }

  resetForm() {
    this.isSubmitting = false;
    this.isSubmitted = false;
    this.bannerPreview = null;
    this.tournamentForm.reset({
      maxTeams: 16,
      entryFee: 0,
      prize: 0
    });
  }
}
