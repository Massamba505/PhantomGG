import { Component, Input, Output, EventEmitter, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateTournament, UpdateTournament, Tournament, TournamentFormat } from '@/app/api/models/tournament.models';
import { TournamentService } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';

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

  tournamentForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(2000)]],
    location: ['', [Validators.maxLength(200)]],
    formatId: ['', [Validators.required]],
    registrationStartDate: [''],
    registrationDeadline: [''],
    startDate: ['', [Validators.required]],
    minTeams: [2, [Validators.required, Validators.min(2), Validators.max(64)]],
    maxTeams: [16, [Validators.required, Validators.min(4), Validators.max(128)]],
    maxPlayersPerTeam: [11, [Validators.required, Validators.min(7), Validators.max(25)]],
    minPlayersPerTeam: [7, [Validators.required, Validators.min(7), Validators.max(25)]],
    entryFee: [0, [Validators.min(0)]],
    prizePool: [0, [Validators.min(0)]],
    bannerUrl: "",
    logoUrl: "",
    contactEmail: ['', [Validators.email]],
    matchDuration: [90, [Validators.required, Validators.min(60), Validators.max(120)]],
    isPublic: [true]
  });

  getTournamentFormats(){
    this.tournamentService.getTournamentFormats().subscribe({
        next:(data)=>{
            this.formats.set(data) 
        }
    })
  }

  ngOnInit() {
    if (this.tournament) {
      this.isEdit.set(true);
      this.populateForm();
      this.currentBannerUrl.set(this.tournament.bannerUrl || null);
      this.currentLogoUrl.set(this.tournament.logoUrl || null);
    }
    this.getTournamentFormats()
  }

  populateForm() {
    if (!this.tournament) return;

    this.tournamentForm.patchValue({
      name: this.tournament.name,
      description: this.tournament.description,
      location: this.tournament.location,
      formatId: this.tournament.formatId,
      registrationStartDate: this.tournament.registrationStartDate ? 
        new Date(this.tournament.registrationStartDate).toISOString().slice(0, 16) : '',
      registrationDeadline: this.tournament.registrationDeadline ? 
        new Date(this.tournament.registrationDeadline).toISOString().slice(0, 16) : '',
      startDate: new Date(this.tournament.startDate).toISOString().slice(0, 16),
      minTeams: this.tournament.minTeams,
      maxTeams: this.tournament.maxTeams,
      maxPlayersPerTeam: this.tournament.maxPlayersPerTeam,
      minPlayersPerTeam: this.tournament.minPlayersPerTeam,
      bannerUrl: this.tournament.bannerUrl || "",
      logoUrl: this.tournament.logoUrl || "",
      entryFee: this.tournament.entryFee || 0,
      prizePool: this.tournament.prizePool || 0,
      contactEmail: this.tournament.contactEmail,
      matchDuration: this.tournament.matchDuration,
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
      formatId: formValue.formatId || undefined,
      registrationStartDate: formValue.registrationStartDate || undefined,
      registrationDeadline: formValue.registrationDeadline || undefined,
      startDate: formValue.startDate || undefined,
      minTeams: formValue.minTeams || undefined,
      maxTeams: formValue.maxTeams || undefined,
      maxPlayersPerTeam: formValue.maxPlayersPerTeam || undefined,
      minPlayersPerTeam: formValue.minPlayersPerTeam || undefined,
      entryFee: formValue.entryFee || undefined,
      prizePool: formValue.prizePool || undefined,
      bannerUrl: formValue.bannerUrl || undefined,
      logoUrl: formValue.logoUrl || undefined,
      contactEmail: formValue.contactEmail || undefined,
      matchDuration: formValue.matchDuration || undefined,
      isPublic: formValue.isPublic ?? undefined
    };

    this.formUpdate.emit(updateData);
  }

  saveNewTournament(formValue:any){
    const createData: CreateTournament = {
      name: formValue.name!,
      description: formValue.description!,
      location: formValue.location || undefined,
      formatId: formValue.formatId!,
      registrationStartDate: formValue.registrationStartDate || undefined,
      registrationDeadline: formValue.registrationDeadline || undefined,
      startDate: formValue.startDate!,
      minTeams: formValue.minTeams!,
      maxTeams: formValue.maxTeams!,
      maxPlayersPerTeam: formValue.maxPlayersPerTeam!,
      minPlayersPerTeam: formValue.minPlayersPerTeam!,
      entryFee: formValue.entryFee || undefined,
      prizePool: formValue.prizePool || undefined,
      contactEmail: formValue.contactEmail || undefined,
      matchDuration: formValue.matchDuration!,
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
    this.tournamentService.uploadOrganizerTournamentBanner(this.tournament.id, file).subscribe({
      next: (response) => {
        this.currentBannerUrl.set(response.bannerUrl);
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
      next: (response) => {
        this.currentLogoUrl.set(response.logoUrl);
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
