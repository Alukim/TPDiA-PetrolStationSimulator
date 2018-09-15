import { TestBed, inject } from '@angular/core/testing';

import { PetrolStationService } from './petrol-station.service';

describe('PetrolStationService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PetrolStationService]
    });
  });

  it('should be created', inject([PetrolStationService], (service: PetrolStationService) => {
    expect(service).toBeTruthy();
  }));
});
