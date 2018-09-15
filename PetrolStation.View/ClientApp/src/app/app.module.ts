import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

import { PetrolStationService } from './petrol-station.service';
import { PetrolStationDetailsComponent } from './petrol-station-details/petrol-station-details.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FetchDataComponent,
    PetrolStationDetailsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: FetchDataComponent, pathMatch: 'full' },
      { path: 'authors', component: HomeComponent },
      { path: 'details/:id', component: PetrolStationDetailsComponent }
    ])
  ],
  providers: [PetrolStationService],
  bootstrap: [AppComponent]
})
export class AppModule { }
