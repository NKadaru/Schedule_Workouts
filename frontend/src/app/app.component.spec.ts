import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    // Flush the health check request triggered by ngOnInit
    fixture.detectChanges();
    httpTesting.expectOne('/api/health').flush({ status: 'healthy', timestamp: '2025-01-15T10:30:00Z' });
    expect(app).toBeTruthy();
  });

  it(`should have the 'frontend' title`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    fixture.detectChanges();
    httpTesting.expectOne('/api/health').flush({ status: 'healthy', timestamp: '2025-01-15T10:30:00Z' });
    expect(app.title).toEqual('frontend');
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    httpTesting.expectOne('/api/health').flush({ status: 'healthy', timestamp: '2025-01-15T10:30:00Z' });
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Welcome to frontend!');
  });

  it('should display health status on successful API response', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    httpTesting.expectOne('/api/health').flush({ status: 'healthy', timestamp: '2025-01-15T10:30:00Z' });
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.health-status')).toBeTruthy();
    expect(compiled.textContent).toContain('healthy');
    expect(compiled.textContent).toContain('2025-01-15T10:30:00Z');
  });

  it('should display error message when API is unreachable', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    httpTesting.expectOne('/api/health').error(new ProgressEvent('error'), { status: 0, statusText: 'Unknown Error' });
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.health-error')).toBeTruthy();
    expect(compiled.textContent).toContain('Unable to connect to the API server');
  });
});
