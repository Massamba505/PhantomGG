export function getFormattedDate(dateString: string, short: boolean = false): string {
if (!dateString) return '';

const date = new Date(dateString);
if (isNaN(date.getTime())) return dateString;

if (short) {
    return new Intl.DateTimeFormat('en-US', { 
    month: 'short', 
    day: 'numeric',
    year: 'numeric'
    }).format(date);
}

return new Intl.DateTimeFormat('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
}).format(date);
}