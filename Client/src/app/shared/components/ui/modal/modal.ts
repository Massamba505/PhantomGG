import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../icons/lucide-icons';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.html',
  styleUrls: ['./modal.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class Modal {
  isOpen = input<boolean>(false);
  title = input<string>('');
  close = output<void>();
  readonly icons = LucideIcons;
}
