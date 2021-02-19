import React, { useCallback } from "react";

import { AutocompleteDialogRow, ColDef, EditableTable, TextDialogRow } from "utils/components";

const DATA_COLUMNS: ColDef[] = [
  { field: "id", headerName: "ID", width: 75 },
  { field: "name", headerName: "Group Name", flex: 1.25 },
  {
    field: "manager",
    headerName: "Manager",
    flex: 1,
    valueGetter: _ => _getUserName(_.row.manager),
  },
];

const EMPTY_GROUP: orch.OptionalId<orch.GroupData> = {
  name: "",
  manager: null as any,
};

interface Props {
  groups?: orch.GroupData[];
  users?: orch.UserData[];
  onGroupChange: (data: orch.OptionalId<orch.GroupPayload>) => Promise<any>;
  onGroupDelete: (groupId: number) => Promise<void>;
}

export default function GroupsTable({ groups, users, onGroupChange, onGroupDelete }: Props) {
  const onEditDone = useCallback(
    async ({ manager, ...value }: orch.OptionalId<orch.GroupData>) => {
      if (!manager?.id) throw { error: "Must select a manager" } as orch.Error;
      return await onGroupChange({ ...value, managerId: manager.id });
    },
    [onGroupChange]
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
            getOptionLabel={_getUserName}
            {...rowProps}
          />
        </>
      )}
    </EditableTable>
  );
}

function _getUserName({ firstName, lastName }: orch.UserData) {
  return `${firstName} ${lastName}`;
}
