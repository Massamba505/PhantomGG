import { PagedResult, PaginationMeta } from './common.models';




export interface PaginationRequest {
  page?: number;
  pageSize?: number;
}

export type { PagedResult, PaginationMeta };
