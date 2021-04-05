import React, { useCallback } from "react";

import Autocomplete from "@material-ui/lab/Autocomplete";
import { EditableTable, ColDef, TextDialogRow, textAutocompleteOptions } from "utils/components";

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
  groups?: orch.GroupData[];
  setGroupFilter: (group?: orch.GroupData) => void;
  onUserChange: (data: orch.OptionalId<orch.UserData>) => Promise<any>;
  onUserDelete: (userId: number) => Promise<void>;
}

export default function UsersTable({
  users,
  groups,
  setGroupFilter,
  onUserChange,
  onUserDelete,
}: Props) {
  const onGroupFilterChange = useCallback(
    (_, group: orch.GroupData | null) => setGroupFilter(group ?? undefined),
    [setGroupFilter]
  );

  return (
    <EditableTable
      rowTypeName="User"
      search
      columns={DATA_COLUMNS}
      rows={users}
      emptyRow={EMPTY_USER}
      onRowChange={onUserChange}
      onRowDelete={onUserDelete}
      filters={
        <Autocomplete
          {...textAutocompleteOptions(groups, { placeholder: "group..." })}
          getOptionLabel={g => g.name}
          onChange={onGroupFilterChange}
        />
      }
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
