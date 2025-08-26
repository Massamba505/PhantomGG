import { NgClass } from '@angular/common';
import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Team, CreateTeamRequest, UpdateTeamRequest } from '../../models/tournament';

@Component({
  selector: 'app-team-form',
  templateUrl: './team-form.html',
  standalone: true,
  imports: [NgClass, ReactiveFormsModule],
})
export class TeamForm implements OnInit, OnChanges {
  @Input() team: Team | null = null;
  @Input() tournamentId: string | null = null;
  @Output() save = new EventEmitter<CreateTeamRequest | UpdateTeamRequest>();
  @Output() cancel = new EventEmitter<void>();

  teamForm!: FormGroup;
  submitted = false;
  logoPreview: string | null = null;

  constructor(private fb: FormBuilder) {
    console.log('TeamForm constructor - team:', this.team)
  }

  ngOnInit() {
    console.log('TeamForm ngOnInit - team:', this.team);
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    console.log('TeamForm ngOnChanges - changes:', changes);
    if (changes['team'] && this.teamForm) {
      console.log('Team changed, reinitializing form with:', changes['team'].currentValue);
      this.initializeForm();
    }
  }

  private initializeForm() {
    console.log('Initializing form with team:', this.team);
    this.teamForm = this.fb.group({
      name: [
        this.team?.name || '',
        [Validators.required, Validators.minLength(2)],
      ],
      manager: [
        this.team?.manager || '',
        [Validators.required, Validators.minLength(2)],
      ],
      numberOfPlayers: [
        this.team?.numberOfPlayers || 1,
        [Validators.required, Validators.min(1), Validators.max(30)],
      ],
      logo: [null], // file control - not required for editing
    });

    // Set logo preview if team has logoUrl
    if (this.team?.logoUrl) {
      this.logoPreview = this.team.logoUrl;
    } else {
      this.logoPreview = null;
    }

    console.log('Form initialized with values:', this.teamForm.value);
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

    // Prepare payload as team request object
    const teamData: CreateTeamRequest | UpdateTeamRequest = {
      name: this.teamForm.value.name,
      manager: this.teamForm.value.manager,
      numberOfPlayers: parseInt(this.teamForm.value.numberOfPlayers, 10),
      logoUrl: this.logoPreview || undefined,
      ...(this.tournamentId ? { tournamentId: this.tournamentId } : {})
    };

    this.save.emit(teamData);
  }

  onCancel() {
    this.cancel.emit();
  }
}
