export interface ApiResponse<T = any> {
  data?: T;
  success: boolean;
  message: string;
  details?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalRecords: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PaginationRequest {
  pageNumber?: number;
  pageSize?: number;
}
