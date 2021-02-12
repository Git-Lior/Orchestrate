import React from "react";

import { EditableTable, ColDef, TextDialogRow } from "utils/components";

const DATA_COLUMNS: ColDef[] = [
  { field: "id", headerName: "ID", width: 75 },
  { field: "firstName", headerName: "First Name", flex: 1 },
  { field: "lastName", headerName: "Last Name", flex: 1 },
  { field: "email", headerName: "Email", flex: 1.5 },
];

const EMPTY_USER: orch.UserData = {
  firstName: "",
  lastName: "",
  email: "",
} as any;

interface Props {
  users?: orch.UserData[];
  onUserChange: (data: orch.OptionalId<orch.UserData>) => Promise<any>;
  onUserDelete: (userId: number) => Promise<void>;
}

export default function UsersTable({ users, onUserChange, onUserDelete }: Props) {
  return (
    <EditableTable
      rowTypeName="User"
      search
      columns={DATA_COLUMNS}
      rows={users}
      emptyRow={EMPTY_USER}
      onRowChange={onUserChange}
      onRowDelete={onUserDelete}
    >
      {rowProps => (
        <>
          <TextDialogRow fieldKey="firstName" label="First Name" {...rowProps} />
          <TextDialogRow fieldKey="lastName" label="Last Name" {...rowProps} />
          <TextDialogRow fieldKey="email" label="Email" {...rowProps} />
        </>
      )}
    </EditableTable>
  );
}
