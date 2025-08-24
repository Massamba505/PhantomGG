import { NgClass } from '@angular/common';
import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-team-form',
  templateUrl: './team-form.html',
  imports: [NgClass, ReactiveFormsModule],
})
export class TeamForm implements OnInit {
  @Input() team: any | null = null;
  @Output() save = new EventEmitter<any>();
  @Output() cancel = new EventEmitter<void>();

  teamForm!: FormGroup;
  submitted = false;
  logoPreview: string | null = null;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.teamForm = this.fb.group({
      name: [
        this.team?.name || '',
        [Validators.required, Validators.minLength(2)],
      ],
      city: [
        this.team?.city || '',
        [Validators.required, Validators.minLength(2)],
      ],
      coach: [
        this.team?.coach || '',
        [Validators.required, Validators.minLength(2)],
      ],
      players: [
        this.team?.players || '',
        [Validators.required, Validators.min(1), Validators.max(30)],
      ],
      logo: [null, Validators.required], // file control
    });

    if (this.team?.logoUrl) {
      this.logoPreview = this.team.logoUrl;
      this.teamForm.patchValue({ logo: this.team.logoUrl });
    }
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
      reader.onload = () => (this.logoPreview = reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  allowDrop(event: DragEvent) {
    event.preventDefault();
  }

  onSubmit() {
    this.submitted = true;
    if (this.teamForm.invalid) {
      return;
    }

    // Prepare payload
    const formData = new FormData();
    formData.append('name', this.teamForm.value.name);
    formData.append('city', this.teamForm.value.city);
    formData.append('coach', this.teamForm.value.coach);
    formData.append('players', this.teamForm.value.players);
    formData.append('logo', this.teamForm.value.logo);

    this.save.emit(formData);
  }

  onCancel() {
    this.cancel.emit();
  }
}
