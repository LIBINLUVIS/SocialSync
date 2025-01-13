import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MySchedulersComponent } from './my-schedulers.component';

describe('MySchedulersComponent', () => {
  let component: MySchedulersComponent;
  let fixture: ComponentFixture<MySchedulersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MySchedulersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MySchedulersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
