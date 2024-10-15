import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_Services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_Services/account.service';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit {
  private accountService = inject(AccountService);
  memberService = inject(MembersService);
  userParams = new UserParams(this.accountService.CurrentUser());
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]

  ngOnInit(): void {
    if(!this.memberService.paginatedResult())
    this.loadMembers();
  }

  resetFilters() {
    this.userParams = new UserParams(this.accountService.CurrentUser());
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams);
  }

  pageChanged(event: any) {
    if(this.userParams.pageNumber != event.page) {
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }

}
