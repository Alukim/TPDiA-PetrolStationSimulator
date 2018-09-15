interface NozzleResponse {
    id: string;
    lastTransactionVolume: number;
    totalStolenPetrolAmount: number;
    lastStolenPetrolAmount: number;
    totalPetrolAmount: number;
}

interface TankResponse {
    id: string;
    maximumVolume: number;
    currentVolume: number;
    petrolTemperature: number;
    tankHigh: number;
    nozzleResponses: NozzleResponse[];
}

interface PetrolStationRaportResponse {
    tankId: string;
    nozzleId: string;
    timeStamp: Date;
    report: string;
    status: string;
}

interface PetrolStationDetailsResponse {
    id: string;
    name: string;
    time: Date;
    tanks: TankResponse[];
    raports: PetrolStationRaportResponse[];
}
