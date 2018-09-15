import { Component, OnInit, OnDestroy } from '@angular/core';
import { PetrolStationService } from '../petrol-station.service';
import { ActivatedRoute } from '../../../node_modules/@angular/router';
import { HubConnection, HubConnectionBuilder } from '../../../node_modules/@aspnet/signalr';

@Component({
  selector: 'app-petrol-station-details',
  templateUrl: './petrol-station-details.component.html',
  styleUrls: ['./petrol-station-details.component.css']
})
export class PetrolStationDetailsComponent implements OnInit {

  public id: string;

  public petrolStation: PetrolStationDetailsResponse;
  public hubConnection: HubConnection;

  private sub: any;

  constructor(private _petrolStationServices: PetrolStationService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });

    this.getPetrolStation();

    let builder = new HubConnectionBuilder();

    // as per setup in the startup.cs
    this.hubConnection = builder.withUrl('/petrolHub').build();

    // message coming from the server
    this.hubConnection.on("StationUpdate", (message) => {
      if(this.id == message)
        this.getPetrolStation()
    });

    // starting the connection
    this.hubConnection.start();
  }

  getPetrolStation() {
    this._petrolStationServices.getById(this.id).subscribe(x => this.petrolStation = x);
  }

  getReport() {
    this._petrolStationServices.getReportById(this.id);
  }


  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}
