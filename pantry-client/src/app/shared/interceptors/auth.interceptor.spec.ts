import { TestBed } from '@angular/core/testing';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthInterceptor } from './auth.interceptor';
import { of, from } from 'rxjs';
import { UserLoginService } from '../services/user-login.service';

describe('AuthInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;

  const oldToken = 'old.jwt.token';
  const newToken = 'new.jwt.token';

  let mockUserService: any;
  let interceptorInstance: AuthInterceptor;

  beforeEach(() => {
    mockUserService = {
      token: oldToken,
      // make refreshToken async so concurrent 401s queue correctly
      refreshToken: jasmine.createSpy('refreshToken').and.callFake((t: string) => from(Promise.resolve({ token: newToken, validTo: new Date().toISOString() }))),
      setToken: jasmine.createSpy('setToken'),
      logout: jasmine.createSpy('logout')
    };

    interceptorInstance = new AuthInterceptor(mockUserService);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        { provide: UserLoginService, useValue: mockUserService },
        // Provide the same interceptor instance so internal state (isRefreshing) is shared
        { provide: HTTP_INTERCEPTORS, useValue: interceptorInstance, multi: true }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('refreshes token on 401 and retries original request', (done) => {
    http.get('/api/test').subscribe((resp: any) => {
      expect(resp).toEqual({ ok: true });
      expect(mockUserService.refreshToken).toHaveBeenCalledWith(oldToken);
      expect(mockUserService.setToken).toHaveBeenCalledWith(newToken, jasmine.any(String));
      done();
    });

    const req1 = httpMock.expectOne('/api/test');
    expect(req1.request.headers.get('Authorization')).toBe(`Bearer ${oldToken}`);

    req1.flush(null, { status: 401, statusText: 'Unauthorized' });

    // wait for async refreshToken to resolve and retry to be issued
    setTimeout(() => {
      const req2 = httpMock.expectOne('/api/test');
      expect(req2.request.headers.get('Authorization')).toBe(`Bearer ${newToken}`);
      req2.flush({ ok: true });
    }, 0);
  });

  it('only calls refresh once for concurrent 401s and retries both requests', (done) => {
    const results: any[] = [];

    http.get('/api/a').subscribe(r => { results.push(r); if (results.length === 2) {
      expect(mockUserService.refreshToken.calls.count()).toBe(1);
      done();
    }});
    http.get('/api/b').subscribe(r => { results.push(r); if (results.length === 2) {
      expect(mockUserService.refreshToken.calls.count()).toBe(1);
      done();
    }});

    const reqA1 = httpMock.expectOne('/api/a');
    const reqB1 = httpMock.expectOne('/api/b');
    expect(reqA1.request.headers.get('Authorization')).toBe(`Bearer ${oldToken}`);
    expect(reqB1.request.headers.get('Authorization')).toBe(`Bearer ${oldToken}`);

    reqA1.flush(null, { status: 401, statusText: 'Unauthorized' });
    reqB1.flush(null, { status: 401, statusText: 'Unauthorized' });

    // wait for async refreshToken to resolve and retries to be issued
    setTimeout(() => {
      const reqA2 = httpMock.expectOne('/api/a');
      const reqB2 = httpMock.expectOne('/api/b');
      expect(reqA2.request.headers.get('Authorization')).toBe(`Bearer ${newToken}`);
      expect(reqB2.request.headers.get('Authorization')).toBe(`Bearer ${newToken}`);

      reqA2.flush({ ok: 'a' });
      reqB2.flush({ ok: 'b' });
    }, 0);
  });

});
