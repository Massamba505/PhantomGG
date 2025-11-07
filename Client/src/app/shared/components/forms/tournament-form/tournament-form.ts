import {
  Component,
  OnInit,
  OnChanges,
  SimpleChanges,
  inject,
  signal,
  computed,
  input,
  output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
  FormGroup,
} from '@angular/forms';
import {
  CreateTournament,
  UpdateTournament,
  Tournament,
} from '@/app/api/models/tournament.models';
import { tournamentDatesValidator } from '@/app/shared/validators/date.validator';

@Component({
  selector: 'app-tournament-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tournament-form.html',
  styleUrl: './tournament-form.css',
})
export class TournamentForm implements OnInit, OnChanges {
  tournament = input<Tournament | null>(null);
  loadingState = input(false);

  formSubmit = output<CreateTournament>();
  formUpdate = output<UpdateTournament>();
  formCancel = output<void>();

  private fb = inject(FormBuilder);

  tournamentForm!: FormGroup;
  submitted = signal(false);
  loading = signal(false);
  bannerPreview = signal<string | null>(null);
  logoPreview = signal<string | null>(null);

  isEditMode = computed(() => this.tournament() !== null);

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
    const t = this.tournament();

    this.tournamentForm = this.fb.group(
      {
        name: [
          t?.name || '',
          [
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(200),
          ],
        ],
        description: [
          t?.description || '',
          [Validators.required, Validators.minLength(10)],
        ],
        location: [
          t?.location || '',
          [Validators.maxLength(200), Validators.required],
        ],
        registrationStartDate: [
          t?.registrationStartDate
            ? new Date(t.registrationStartDate).toISOString().slice(0, 16)
            : '',
          [Validators.required],
        ],
        registrationDeadline: [
          t?.registrationDeadline
            ? new Date(t.registrationDeadline).toISOString().slice(0, 16)
            : '',
          [Validators.required],
        ],
        startDate: [
          t?.startDate ? new Date(t.startDate).toISOString().slice(0, 16) : '',
          [Validators.required],
        ],
        endDate: [
          t?.endDate ? new Date(t.endDate).toISOString().slice(0, 16) : '',
          [Validators.required],
        ],
        minTeams: [
          t?.minTeams || 2,
          [Validators.required, Validators.min(2), Validators.max(64)],
        ],
        maxTeams: [
          t?.maxTeams || 16,
          [Validators.required, Validators.min(4), Validators.max(128)],
        ],
        isPublic: [t?.isPublic ?? true],
        banner: [null],
        logo: [null],
      },
      { validators: [tournamentDatesValidator()] }
    );

    this.bannerPreview.set(t?.bannerUrl ?? null);
    this.logoPreview.set(t?.logoUrl ?? null);
  }

  onBannerChange(event: Event) {
    const file = (event.target as HTMLInputElement)?.files?.[0];
    if (file) {
      this.tournamentForm.patchValue({ banner: file });
      const reader = new FileReader();
      reader.onload = () => this.bannerPreview.set(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  onLogoChange(event: Event) {
    const file = (event.target as HTMLInputElement)?.files?.[0];
    if (file) {
      this.tournamentForm.patchValue({ logo: file });
      const reader = new FileReader();
      reader.onload = () => this.logoPreview.set(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  onSubmit() {
    this.submitted.set(true);
    if (this.tournamentForm.invalid) return;

    const f = this.tournamentForm.value;

    if (this.isEditMode()) {
      const data: UpdateTournament = {
        name: f.name,
        description: f.description,
        location: f.location || undefined,
        registrationStartDate: f.registrationStartDate || undefined,
        registrationDeadline: f.registrationDeadline || undefined,
        startDate: f.startDate || undefined,
        endDate: f.endDate || undefined,
        minTeams: f.minTeams || undefined,
        maxTeams: f.maxTeams || undefined,
        isPublic: f.isPublic ?? undefined,
        bannerUrl: f.banner instanceof File ? f.banner : undefined,
        logoUrl: f.logo instanceof File ? f.logo : undefined,
      };
      this.formUpdate.emit(data);
    } else {
      const data: CreateTournament = {
        name: f.name,
        description: f.description,
        location: f.location || undefined,
        registrationStartDate: f.registrationStartDate || undefined,
        registrationDeadline: f.registrationDeadline || undefined,
        startDate: f.startDate,
        endDate: f.endDate,
        minTeams: f.minTeams,
        maxTeams: f.maxTeams,
        isPublic: f.isPublic ?? true,
        bannerUrl: f.banner instanceof File ? f.banner : undefined,
        logoUrl: f.logo instanceof File ? f.logo : undefined,
      };
      this.formSubmit.emit(data);
    }
  }

  onCancel() {
    this.formCancel.emit();
  }

  isFieldInvalid(name: string): boolean {
    const ctrl = this.tournamentForm.get(name);
    return !!(ctrl && ctrl.invalid && this.submitted());
  }
}
