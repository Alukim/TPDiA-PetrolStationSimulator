import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PetrolStationService {

  public url: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.url = baseUrl;
  }

  getData(): Observable<PetrolStationResponse[]> {
    return this.http.get<PetrolStationResponse[]>(this.url + 'petrolStation');
  }

  getById(id: string) : Observable<PetrolStationDetailsResponse> {
    return this.http.get<PetrolStationDetailsResponse>(`${this.url}petrolStation/${id}`);
  }

  getReportById(id: string) {
    this.http.get(`${this.url}petrolStation/${id}/report`, { responseType: 'blob', observe: 'response' })
      .subscribe(resp => this.downLoadFile(resp, "application/ms-excel", resp.headers.get('Content-Disposition')));
  }

  downLoadFile(data: any, type: string, fileName: string) {
    var result = fileName.split(';')[1].trim().split('=')[1];
    let formatedFileName = result.replace(/"/g, '');
    console.log(formatedFileName);
    let blob = new Blob([data], { type: type });
    let link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = formatedFileName;
    link.click();
  }
}
