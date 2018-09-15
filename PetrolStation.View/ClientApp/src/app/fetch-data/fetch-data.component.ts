import { Component, OnInit} from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { PetrolStationService } from '../petrol-station.service';


@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public petrolStations: PetrolStationResponse[];
  public hubConnection: HubConnection;

  constructor(private _petrolStationServices: PetrolStationService) {}

  ngOnInit() {
    this._petrolStationServices.getData().subscribe(x => this.petrolStations = x);

    let builder = new HubConnectionBuilder();

    // as per setup in the startup.cs
    this.hubConnection = builder.withUrl('/petrolHub').build();

    // message coming from the server
    this.hubConnection.on("NewStation", (message) => {
      this.reloadStations()
    });

    // starting the connection
    this.hubConnection.start();
  }

  reloadStations() {
    this._petrolStationServices.getData().subscribe(x => this.petrolStations = x);
  }

  details(id: string) {
    console.log(id);
  }
}
