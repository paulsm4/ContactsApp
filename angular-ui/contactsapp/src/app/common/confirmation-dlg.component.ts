import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  template: `
  <h3 mat-dialog-title>{{dlgTitle}}</h3>
  <mat-dialog-content>{{dlgMessage}}</mat-dialog-content>
  <mat-dialog-actions>
    <button mat-button mat-dialog-close>No</button>
    <button mat-button [mat-dialog-close]="true" cdkFocusInitial>Yes</button>
  </mat-dialog-actions>
  `
})
export class ConfirmationDlgComponent {
  dlgTitle: string;
  dlgMessage: string;
  // defaultConfig: MatDialogConfig = {
  //   hasBackdrop: true,
  //   height: '250px',
  //   position: {top: '', bottom: '', left: '', right: ''}
  // };

  constructor(
    public dialogRef: MatDialogRef<ConfirmationDlgComponent>,
    @Inject(MAT_DIALOG_DATA) public extraData) {
    console.log('ConfirmationDlgComponent(), extraData:', extraData);
    this.dlgTitle = extraData.dlgTitle;
    this.dlgMessage = extraData.dlgMessage;
  }

  // constructor(public dialog: MatDialog) { }

  // openDialog(title: string, message: string): void {
  //   this.dlgTitle = title;
  //   this.dlgMessage = message;
  //   let dialogRef = this.dialog.open(ConfirmationDlgComponent, this.defaultConfig);
  // }
}
