import React, { useCallback } from "react";

import { AutocompleteDialogRow, ColDef, EditableTable, TextDialogRow } from "utils/components";

const DATA_COLUMNS: ColDef[] = [
  { field: "id", headerName: "ID", width: 75 },
  { field: "name", headerName: "Group Name", flex: 1.25 },
  { field: "manager", headerName: "Manager", flex: 1 },
];

const EMPTY_GROUP: orch.GroupData = {
  name: "",
  manager: undefined,
} as any;

interface Props {
  groups?: orch.GroupData[];
  users?: orch.UserData[];
  onGroupAdd: (data: orch.GroupData) => Promise<void>;
  onGroupChange: (data: orch.GroupData) => Promise<void>;
  onGroupDelete: (data: orch.GroupData) => Promise<void>;
}

export default function GroupsTable({
  groups,
  users,
  onGroupAdd,
  onGroupChange,
  onGroupDelete,
}: Props) {
  const onEditDone = useCallback(
    (value: orch.GroupData) => (value.id ? onGroupChange(value) : onGroupAdd(value)),
    [onGroupAdd, onGroupChange]
  );

  const getUserName = useCallback(
    (user: orch.UserData) => `${user.firstName} ${user.lastName}`,
    []
  );

  return (
    <EditableTable
      rowTypeName="Group"
      search
      columns={DATA_COLUMNS}
      rows={groups}
      emptyRow={EMPTY_GROUP}
      onRowChange={onEditDone}
      onRowDelete={onGroupDelete}
    >
      {rowProps => (
        <>
          <TextDialogRow fieldKey="name" label="Group Name" {...rowProps} />
          <AutocompleteDialogRow
            fieldKey="manager"
            label="Manager"
            options={users}
            getOptionLabel={getUserName}
            {...rowProps}
          />
        </>
      )}
    </EditableTable>
  );
}
