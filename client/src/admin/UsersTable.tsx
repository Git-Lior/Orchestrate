import React, { useCallback } from "react";

import { EditableTable, ColDef, TextDialogRow } from "utils/components";

const DATA_COLUMNS: ColDef[] = [
  { field: "id", headerName: "ID", width: 75 },
  { field: "firstName", headerName: "First Name", width: 125 },
  { field: "lastName", headerName: "Last Name", width: 125 },
  { field: "email", headerName: "Email", width: 175 },
];

const EMPTY_USER: orch.UserData = {
  id: NaN,
  firstName: "",
  lastName: "",
  email: "",
};

interface Props {
  users?: orch.UserData[];
  onUserAdd: (data: orch.UserData) => Promise<void>;
  onUserChange: (data: orch.UserData) => Promise<void>;
  onUserDelete: (data: orch.UserData) => Promise<void>;
}

export default function UsersTable({ users, onUserAdd, onUserChange, onUserDelete }: Props) {
  const onEditDone = useCallback(
    (value: orch.UserData) => (value.id ? onUserChange(value) : onUserAdd(value)),
    [onUserAdd, onUserChange]
  );

  return (
    <EditableTable
      rowTypeName="User"
      search
      columns={DATA_COLUMNS}
      rows={users}
      emptyRow={EMPTY_USER}
      onRowChange={onEditDone}
      onRowDelete={onUserDelete}
    >
      {({ value, onChange }) => (
        <>
          <TextDialogRow
            fieldKey="firstName"
            label="First Name"
            value={value}
            onChange={onChange}
          />
          <TextDialogRow fieldKey="lastName" label="Last Name" value={value} onChange={onChange} />
          <TextDialogRow fieldKey="email" label="Email" value={value} onChange={onChange} />
        </>
      )}
    </EditableTable>
  );
}
