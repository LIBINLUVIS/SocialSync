import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResetpassowordComponent } from './resetpassoword.component';

describe('ResetpassowordComponent', () => {
  let component: ResetpassowordComponent;
  let fixture: ComponentFixture<ResetpassowordComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResetpassowordComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResetpassowordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
