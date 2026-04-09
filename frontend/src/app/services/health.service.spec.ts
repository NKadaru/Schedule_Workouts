import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HealthService } from './health.service';
import { HealthResponse } from '../models/health-response';

/**
 * Unit tests for HealthService
 * Validates: Requirements 5.1
 */
describe('HealthService', () => {
  let service: HealthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        HealthService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(HealthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call GET /api/health and return HealthResponse', () => {
    const mockResponse: HealthResponse = {
      status: 'healthy',
      timestamp: '2025-01-15T10:30:00Z'
    };

    service.checkHealth().subscribe(response => {
      expect(response).toEqual(mockResponse);
      expect(response.status).toBe('healthy');
      expect(response.timestamp).toBe('2025-01-15T10:30:00Z');
    });

    const req = httpTesting.expectOne('/api/health');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should propagate error when API returns non-2xx response', () => {
    const errorMessage = 'Internal Server Error';

    service.checkHealth().subscribe({
      next: () => fail('Expected an error, but got a response'),
      error: (error) => {
        expect(error.status).toBe(500);
        expect(error.statusText).toBe(errorMessage);
      }
    });

    const req = httpTesting.expectOne('/api/health');
    expect(req.request.method).toBe('GET');
    req.flush(null, { status: 500, statusText: errorMessage });
  });

  it('should propagate error when API is unreachable', () => {
    service.checkHealth().subscribe({
      next: () => fail('Expected an error, but got a response'),
      error: (error) => {
        expect(error.status).toBe(0);
      }
    });

    const req = httpTesting.expectOne('/api/health');
    expect(req.request.method).toBe('GET');
    req.error(new ProgressEvent('error'), { status: 0, statusText: 'Unknown Error' });
  });
});
