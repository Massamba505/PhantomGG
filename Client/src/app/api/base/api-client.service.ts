import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PagedResult } from '../models/api.models';
import { environment } from '@/environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class ApiClient {
  private readonly http = inject(HttpClient);
  private readonly env = environment;
  private readonly baseUrl = this.env.apiUrl;

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`);
  }

  post<T>(endpoint: string, data?: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, data);
  }

  put<T>(endpoint: string, data?: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${endpoint}`, data);
  }

  patch<T>(endpoint: string, data?: any): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}/${endpoint}`, data);
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}/${endpoint}`, {
      observe: 'response'
    }).pipe(
      map(response => {
        if (response.status === 204) {
          return null as T;
        }
        
        return response.body as T;
      })
    );
  }

  private convertSearchParams(params: any): any {
    if (!params) return params;
    
    const converted = { ...params };
    
    if (converted.searchTerm) {
      converted.q = converted.searchTerm;
      delete converted.searchTerm;
    }
    
    if (converted.pageNumber) {
      converted.page = converted.pageNumber;
      delete converted.pageNumber;
    }
    return converted;
  }
  
  getPaged<T>(endpoint: string, params?: any): Observable<PagedResult<T>> {
    let httpParams = new HttpParams();
    const convertedParams = this.convertSearchParams(params);
    
    if (convertedParams) {
      Object.keys(convertedParams).forEach(key => {
        if (convertedParams[key] !== null && convertedParams[key] !== undefined) {
          httpParams = httpParams.set(key, convertedParams[key].toString());
        }
      });
    }

    return this.http.get<PagedResult<T>>(
      `${this.baseUrl}/${endpoint}`, 
      { params: httpParams }
    ).pipe(
      map(result => ({
        ...result,
        totalRecords: result.meta.totalRecords
      }))
    );
  }

  uploadFile<T>(endpoint: string, file: File): Observable<T> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, formData);
  }

  postFormData<T>(endpoint: string, formData: FormData): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, formData);
  }

  putFormData<T>(endpoint: string, formData: FormData): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${endpoint}`, formData);
  }

  patchFormData<T>(endpoint: string, formData: FormData): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}/${endpoint}`, formData);
  }
}
