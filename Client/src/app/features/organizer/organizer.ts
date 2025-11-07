import { OrganizerLayout } from '@/app/shared/components/layouts/organizer-layout/organizer-layout';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-organizer',
  imports: [RouterOutlet, OrganizerLayout],
  templateUrl: './organizer.html',
})
export class OrganizerComponent {}
