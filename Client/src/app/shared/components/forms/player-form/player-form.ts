import { Component, input, output, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Player, CreatePlayer, UpdatePlayer } from '@/app/api/models/team.models';

@Component({
  selector: 'app-player-form',
  templateUrl: './player-form.html',
  styleUrls: ['./player-form.css'],
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
})
export class PlayerForm implements OnInit {
  private fb = inject(FormBuilder);
  
  player = input<Player | null>(null); // null for create, Player for edit
  teamId = input.required<string>();
  
  save = output<CreatePlayer | UpdatePlayer>();
  cancel = output<void>();
  
  readonly icons = LucideIcons;
  
  form!: FormGroup;
  isEditMode = false;
  selectedFile = signal<File | null>(null);
  previewUrl = signal<string | null>(null);

  // Computed preview data for the player card
  previewData = computed(() => {
    if (!this.form) return null;
    
    const formValue = this.form.value;
    const currentPlayer = this.player();
    
    return {
      firstName: formValue.firstName || 'First',
      lastName: formValue.lastName || 'Last',
      position: formValue.position || undefined,
      email: formValue.email || undefined,
      photoUrl: this.previewUrl() || currentPlayer?.photoUrl,
      joinedAt: currentPlayer?.joinedAt || new Date().toISOString()
    };
  });

  ngOnInit() {
    this.isEditMode = !!this.player();
    this.initializeForm();
    
    // Set initial preview URL for existing player
    const currentPlayer = this.player();
    if (currentPlayer?.photoUrl) {
      this.previewUrl.set(currentPlayer.photoUrl);
    }
  }

  initializeForm() {
    const player = this.player();
    
    this.form = this.fb.group({
      firstName: [player?.firstName || '', [Validators.required, Validators.maxLength(100)]],
      lastName: [player?.lastName || '', [Validators.required, Validators.maxLength(100)]],
      position: [player?.position || '', [Validators.maxLength(30)]],
      email: [player?.email || '', [Validators.email, Validators.maxLength(100)]],
      photoUrl: [null]
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const formValue = this.form.value;
      
      if (this.isEditMode) {
        const updateData: UpdatePlayer = {
          firstName: formValue.firstName,
          lastName: formValue.lastName,
          position: formValue.position || undefined,
          email: formValue.email || undefined,
          photoUrl: formValue.photoUrl || undefined
        };
        this.save.emit(updateData);
      } else {
        const createData: CreatePlayer = {
          firstName: formValue.firstName,
          lastName: formValue.lastName,
          position: formValue.position || undefined,
          email: formValue.email || undefined,
          photoUrl: formValue.photoUrl || undefined,
          teamId: this.teamId()
        };
        this.save.emit(createData);
      }
    }
  }

  onCancel() {
    this.cancel.emit();
  }

  onFileSelect(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.selectedFile.set(file);
      this.form.patchValue({ photoUrl: file });
      
      // Create preview URL
      const reader = new FileReader();
      reader.onload = (e) => {
        this.previewUrl.set(e.target?.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  getPlayerInitials(): string {
    const preview = this.previewData();
    if (!preview) return 'FL';
    return `${preview.firstName.charAt(0)}${preview.lastName.charAt(0)}`.toUpperCase();
  }

  getFullName(): string {
    const preview = this.previewData();
    if (!preview) return 'First Last';
    return `${preview.firstName} ${preview.lastName}`;
  }

  getTitle(): string {
    return this.isEditMode ? 'Edit Player' : 'Add New Player';
  }

  getSubmitText(): string {
    return this.isEditMode ? 'Update Player' : 'Add Player';
  }

  getFieldError(fieldName: string): string | null {
    const field = this.form.get(fieldName);
    if (field && field.invalid && (field.dirty || field.touched)) {
      const errors = field.errors;
      if (errors?.['required']) return `${this.getFieldLabel(fieldName)} is required`;
      if (errors?.['email']) return 'Please enter a valid email address';
      if (errors?.['maxlength']) return `${this.getFieldLabel(fieldName)} is too long`;
    }
    return null;
  }

  private getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      firstName: 'First name',
      lastName: 'Last name',
      position: 'Position',
      email: 'Email'
    };
    return labels[fieldName] || fieldName;
  }
}