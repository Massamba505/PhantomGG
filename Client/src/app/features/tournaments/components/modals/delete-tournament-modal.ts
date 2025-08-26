import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-delete-tournament-modal',
  standalone: true,
  imports: [Modal, LucideAngularModule],
  template: `
    <app-modal 
      [isOpen]="isOpen" 
      (close)="close.emit()" 
      title="Delete Tournament"
    >
      <div class="space-y-4">
        <div class="flex items-center space-x-3">
          <div class="flex-shrink-0">
            <lucide-angular 
              [img]="icons.AlertTriangle" 
              class="w-6 h-6 text-red-500"
            ></lucide-angular>
          </div>
          <div class="flex-1">
            <p class="text-sm text-gray-900">
              Are you sure you want to delete 
              @if (tournamentName) {
                <strong>"{{ tournamentName }}"</strong>
              } @else {
                this tournament
              }?
            </p>
            <p class="text-sm text-gray-500 mt-1">
              This action cannot be undone. All tournament data, teams, and results will be permanently removed.
            </p>
          </div>
        </div>

        <div class="flex justify-end space-x-3 pt-4">
          <button
            type="button"
            (click)="close.emit()"
            class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
          >
            Cancel
          </button>
          <button
            type="button"
            (click)="onConfirm()"
            class="px-4 py-2 text-sm font-medium text-white bg-red-600 border border-transparent rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
          >
            Delete Tournament
          </button>
        </div>
      </div>
    </app-modal>
  `
})
export class DeleteTournamentModal {
  @Input() isOpen = false;
  @Input() tournamentName: string | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<void>();

  readonly icons = LucideIcons;

  onConfirm() {
    this.confirm.emit();
    this.close.emit();
  }
}
