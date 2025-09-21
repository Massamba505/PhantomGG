import { Component, Input, Output, EventEmitter, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { CreateTournament, UpdateTournament, Tournament, TournamentFormat } from '@/app/api/models/tournament.models';
import { TournamentService } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';
import { dateNotInPastValidator, registrationDeadlineValidator, tournamentStartDateValidator } from '@/app/shared/validators/date.validator';

@Component({
  selector: 'app-tournament-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tournament-form.component.html',
  styleUrl: './tournament-form.component.css'
})
export class TournamentFormComponent implements OnInit {
  @Input() tournament: Tournament | null = null;
  @Input() set loadingState(value: boolean) {
    this.loading.set(value);
  }
  @Output() formSubmit = new EventEmitter<CreateTournament>();
  @Output() formUpdate = new EventEmitter<UpdateTournament>();
  @Output() formCancel = new EventEmitter<void>();
  private readonly tournamentService = inject(TournamentService);
  private readonly toastService = inject(ToastService);

  private fb = inject(FormBuilder);
  
  loading = signal(false);
  isEdit = signal(false);
  uploadingBanner = signal(false);
  uploadingLogo = signal(false);
  bannerPreview = signal<string | null>(null);
  logoPreview = signal<string | null>(null);
  
  formats = signal<TournamentFormat[]>([]);
  currentBannerUrl = signal<string | null>(null);
  currentLogoUrl = signal<string | null>(null);
  
  get todayDateTime(): string {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  }

  tournamentForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(2000)]],
    location: ['', [Validators.maxLength(200)]],
    registrationStartDate: ['', [
      Validators.required, 
      dateNotInPastValidator
    ]],
    registrationDeadline: ['', [
      Validators.required, 
      dateNotInPastValidator,
      registrationDeadlineValidator
    ]],
    startDate: ['', [
      Validators.required, 
      dateNotInPastValidator,
      tournamentStartDateValidator
    ]],
    endDate: ['', [Validators.required]],
    minTeams: [2, [Validators.required, Validators.min(2), Validators.max(64)]],
    maxTeams: [16, [Validators.required, Validators.min(4), Validators.max(128)]],
    bannerUrl: [""],
    logoUrl: [""],
    contactEmail: ['', [Validators.email]],
    isPublic: [true]
  });


  getTournamentFormats(){
    debugger;
    this.tournamentService.getTournamentFormats().subscribe({
        next:(data: any)=>{
            this.formats.set(data) 
        }
    });

  }

  ngOnInit() {
    this.setupCrossFieldValidation();
    
    if (this.tournament) {
      this.isEdit.set(true);
      this.populateForm();
      this.currentBannerUrl.set(this.tournament.bannerUrl || null);
      this.currentLogoUrl.set(this.tournament.logoUrl || null);
    }
    this.getTournamentFormats();
  }

  private setupCrossFieldValidation() {
      let isValidationRunning = false;

      this.tournamentForm.get('registrationStartDate')?.valueChanges.subscribe(() => {
        if (isValidationRunning) return;
        isValidationRunning = true;
        this.tournamentForm.get('registrationDeadline')?.updateValueAndValidity({ onlySelf: true });
        isValidationRunning = false;
      });

      this.tournamentForm.get('registrationDeadline')?.valueChanges.subscribe(() => {
        if (isValidationRunning) return;
        isValidationRunning = true;
        this.tournamentForm.get('startDate')?.updateValueAndValidity({ onlySelf: true });
        isValidationRunning = false;
      });

      this.tournamentForm.get('startDate')?.valueChanges.subscribe(() => {
        if (isValidationRunning) return;
        isValidationRunning = true;
        this.tournamentForm.get('registrationDeadline')?.updateValueAndValidity({ onlySelf: true });
        isValidationRunning = false;
      });
  }


  hasFieldError(fieldName: string, errorType: string): boolean {
    const field = this.tournamentForm.get(fieldName);
    return !!(field?.hasError(errorType) && (field?.dirty || field?.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.tournamentForm.get(fieldName);
    if (!field || !field.errors || !field.touched) return '';

    if (field.hasError('required')) {
      return `${fieldName.replace(/([A-Z])/g, ' $1').toLowerCase()} is required`;
    }
    if (field.hasError('dateInPast')) {
      return 'Date cannot be in the past';
    }
    if (field.hasError('deadlineBeforeStart')) {
      return 'Registration deadline must be after registration start';
    }
    if (field.hasError('deadlineAfterTournamentStart')) {
      return 'Registration deadline must be before tournament start';
    }
    if (field.hasError('startBeforeDeadline')) {
      return 'Tournament start must be after registration deadline';
    }
    
    return '';
  }

  populateForm() {
    if (!this.tournament) return;

    this.tournamentForm.patchValue({
      name: this.tournament.name,
      description: this.tournament.description,
      location: this.tournament.location,
      registrationStartDate: this.tournament.registrationStartDate ? 
        new Date(this.tournament.registrationStartDate).toISOString().slice(0, 16) : '',
      registrationDeadline: this.tournament.registrationDeadline ? 
        new Date(this.tournament.registrationDeadline).toISOString().slice(0, 16) : '',
      startDate: new Date(this.tournament.startDate).toISOString().slice(0, 16),
      endDate: new Date(this.tournament.endDate).toISOString().slice(0, 16),
      minTeams: this.tournament.minTeams,
      maxTeams: this.tournament.maxTeams,
      bannerUrl: this.tournament.bannerUrl || "",
      logoUrl: this.tournament.logoUrl || "",
      contactEmail: this.tournament.organizer?.email || '',
      isPublic: this.tournament.isPublic
    });
  }

  onSubmit() {
    if (this.tournamentForm.valid) {
      const formValue = this.tournamentForm.value;
      
      if (this.isEdit()) {
        this.saveUpdates(formValue)
      } else {
        this.saveNewTournament(formValue);
      }
    }
  }

  saveUpdates(formValue:any){
    const updateData: UpdateTournament = {
      name: formValue.name || undefined,
      description: formValue.description || undefined,
      location: formValue.location || undefined,
      registrationStartDate: formValue.registrationStartDate || undefined,
      registrationDeadline: formValue.registrationDeadline || undefined,
      startDate: formValue.startDate || undefined,
      endDate: formValue.endDate || undefined,
      minTeams: formValue.minTeams || undefined,
      maxTeams: formValue.maxTeams || undefined,
      bannerUrl: formValue.bannerUrl || undefined,
      logoUrl: formValue.logoUrl || undefined,
      contactEmail: formValue.contactEmail || undefined,
      isPublic: formValue.isPublic ?? undefined
    };

    this.formUpdate.emit(updateData);
  }

  saveNewTournament(formValue:any){
    const createData: CreateTournament = {
      name: formValue.name!,
      description: formValue.description!,
      location: formValue.location || undefined,
      registrationStartDate: formValue.registrationStartDate || undefined,
      registrationDeadline: formValue.registrationDeadline || undefined,
      startDate: formValue.startDate!,
      endDate: formValue.endDate!,
      minTeams: formValue.minTeams!,
      maxTeams: formValue.maxTeams!,
      contactEmail: formValue.contactEmail || undefined,
      isPublic: formValue.isPublic ?? true
    };

    this.formSubmit.emit(createData);
  }

  onCancel() {
    this.formCancel.emit();
  }

  onBannerSelected(files: File[]) {
    if (files.length > 0 && this.tournament?.id) {
      this.uploadBanner(files[0]);
    }
  }

  onLogoSelected(files: File[]) {
    if (files.length > 0 && this.tournament?.id) {
      this.uploadLogo(files[0]);
    }
  }

  private uploadBanner(file: File) {
    if (!this.tournament?.id) {
      this.toastService.error('Tournament ID is required for banner upload');
      return;
    }

    this.uploadingBanner.set(true);
    this.tournamentService.uploadTournamentBanner(this.tournament.id, file).subscribe({
      next: (response: any) => {
        this.currentBannerUrl.set(response.imageUrl);
        this.tournamentForm.patchValue({bannerUrl:this.currentBannerUrl()})
        this.toastService.success('Banner uploaded successfully');
        this.uploadingBanner.set(false);
      }
    });
  }

  private uploadLogo(file: File) {
    if (!this.tournament?.id) {
      this.toastService.error('Tournament ID is required for logo upload');
      return;
    }

    this.uploadingLogo.set(true);
    this.tournamentService.uploadTournamentLogo(this.tournament.id, file).subscribe({
      next: (response: any) => {
        this.currentLogoUrl.set(response.imageUrl);
        this.tournamentForm.patchValue({logoUrl:this.currentLogoUrl()})
        this.toastService.success('Logo uploaded successfully');
        this.uploadingLogo.set(false);
      }
    });
  }

  onBannerUploadError(event: { file: File; error: string }) {
    this.toastService.error(`Banner upload error: ${event.error}`);
  }

  onLogoUploadError(event: { file: File; error: string }) {
    this.toastService.error(`Logo upload error: ${event.error}`);
  }

  onBannerChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      const reader = new FileReader();
      reader.onload = (e) => {
        this.bannerPreview.set(e.target?.result as string);
      };
      reader.readAsDataURL(file);

      if (this.tournament?.id) {
        this.uploadBanner(file);
      }
    }
  }

  onLogoChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      const reader = new FileReader();
      reader.onload = (e) => {
        this.logoPreview.set(e.target?.result as string);
      };
      reader.readAsDataURL(file);

      if (this.tournament?.id) {
        this.uploadLogo(file);
      }
    }
  }
}
