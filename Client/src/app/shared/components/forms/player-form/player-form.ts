import {
  Component,
  input,
  output,
  OnInit,
  inject,
  signal,
  computed,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../../ui/icons/lucide-icons';
import {
  Player,
  CreatePlayer,
  UpdatePlayer,
} from '@/app/api/models/team.models';
import { PlayerPosition } from '@/app/api/models/common.models';
import { getEnumOptions, getEnumLabel } from '@/app/shared/utils/enumConvertor';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';

@Component({
  selector: 'app-player-form',
  templateUrl: './player-form.html',
  styleUrls: ['./player-form.css'],
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
})
export class PlayerForm implements OnInit {
  private readonly fb = inject(FormBuilder);

  player = input<Player | null>(null);
  teamId = input.required<string>();

  save = output<CreatePlayer | UpdatePlayer>();
  cancel = output<void>();

  readonly icons = LucideIcons;

  form!: FormGroup;
  isEditMode = false;
  selectedFile = signal<File | null>(null);
  previewUrl = signal<string | null>(null);

  positionOptions = getEnumOptions(PlayerPosition);

  constructor() {
    effect(() => {
      const currentPlayer = this.player();
      this.isEditMode = !!currentPlayer;
      debugger;
      if (this.form?.controls) {
        if (currentPlayer) {
          this.form.patchValue({
            firstName: currentPlayer.firstName || '',
            lastName: currentPlayer.lastName || '',
            position: currentPlayer.position?.toString() || '',
            email: currentPlayer.email || '',
          });

          if (currentPlayer.photoUrl) {
            this.previewUrl.set(currentPlayer.photoUrl);
          }
        } else {
          this.form.patchValue({
            firstName: '',
            lastName: '',
            position: '',
            email: '',
          });
          this.previewUrl.set(null);
        }

        this.selectedFile.set(null);
        this.form.patchValue({ photoUrl: null });
      }
    });
  }

  previewData = computed(() => {
    if (!this.form) return null;

    const formValue = this.form.value;
    const currentPlayer = this.player();

    return {
      firstName: formValue.firstName || 'First',
      lastName: formValue.lastName || 'Last',
      position: this.getPositionLabel(formValue.position),
      email: formValue.email || undefined,
      photoUrl: this.previewUrl() || currentPlayer?.photoUrl,
      joinedAt: currentPlayer?.joinedAt || new Date().toISOString(),
    };
  });

  ngOnInit() {
    this.initializeForm();
  }

  initializeForm() {
    const player = this.player();

    this.form = this.fb.group({
      firstName: [
        player?.firstName || '',
        [Validators.required, Validators.maxLength(100)],
      ],
      lastName: [
        player?.lastName || '',
        [Validators.required, Validators.maxLength(100)],
      ],
      position: [player?.position?.toString() || '', [Validators.required]],
      email: [
        player?.email || '',
        [Validators.email, Validators.maxLength(100), strictEmailValidator],
      ],
      photoUrl: [null],
    });
  }

  resetForm() {
    if (this.form) {
      this.form.reset();
    }
    this.selectedFile.set(null);
    this.previewUrl.set(null);
  }

  clearFormData() {
    if (this.form) {
      this.form.reset();
    }
    this.selectedFile.set(null);
    this.previewUrl.set(null);
  }

  getPositionLabel(position: number | string): string | undefined {
    if (!position) return undefined;
    const positionValue =
      typeof position === 'string' ? parseInt(position) : position;
    return getEnumLabel(PlayerPosition, positionValue);
  }

  onSubmit() {
    if (this.form.valid) {
      const formValue = this.form.value;

      if (this.isEditMode) {
        const updateData: UpdatePlayer = {
          firstName: formValue.firstName,
          lastName: formValue.lastName,
          position: formValue.position
            ? parseInt(formValue.position)
            : undefined,
          email: formValue.email || undefined,
          photoUrl: formValue.photoUrl || undefined,
        };
        this.save.emit(updateData);
      } else {
        const createData: CreatePlayer = {
          firstName: formValue.firstName,
          lastName: formValue.lastName,
          position: formValue.position || undefined,
          email: formValue.email || undefined,
          photoUrl: formValue.photoUrl || undefined,
          teamId: this.teamId(),
        };
        this.save.emit(createData);
      }
      this.clearFormData();
    }
  }

  onCancel() {
    this.clearFormData();
    this.cancel.emit();
  }

  onFileSelect(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.[0]) {
      const file = input.files[0];
      this.selectedFile.set(file);
      this.form.patchValue({ photoUrl: file });

      const reader = new FileReader();
      reader.onload = (e) => {
        this.previewUrl.set(e.target?.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  getFullName(): string {
    const preview = this.previewData();
    if (!preview) return 'First Last';
    return `${preview.firstName} ${preview.lastName}`;
  }

  getSubmitText(): string {
    return this.isEditMode ? 'Update Player' : 'Add Player';
  }

  getFieldError(fieldName: string): string | null {
    const field = this.form.get(fieldName);
    if (field && field.invalid && (field.dirty || field.touched)) {
      const errors = field.errors;
      if (errors?.['required'])
        return `${this.getFieldLabel(fieldName)} is required`;
      if (errors?.['email'] || errors?.['invalidEmail'])
        return 'Please enter a valid email address';
      if (errors?.['maxlength'])
        return `${this.getFieldLabel(fieldName)} is too long`;
    }
    return null;
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      firstName: 'First name',
      lastName: 'Last name',
      position: 'Position',
      email: 'Email',
    };
    return labels[fieldName] || fieldName;
  }
}
