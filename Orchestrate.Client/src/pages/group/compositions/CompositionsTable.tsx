import React, { useCallback, useMemo } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";
import IconButton from "@material-ui/core/IconButton";
import DeleteIcon from "@material-ui/icons/Delete";
import EditIcon from "@material-ui/icons/Edit";

import { DataGrid, ColDef, RowParams, CellParams } from "@material-ui/data-grid";

const DATA_COLUMNS: ColDef[] = [
  { field: "title", headerName: "Title", flex: 3 },
  { field: "composer", headerName: "Composer", flex: 1 },
  { field: "genre", headerName: "Genre", flex: 1 },
  { field: "uploader", headerName: "Uploaded by", flex: 1 },
];

const useStyles = makeStyles({
  compositionsTable: {
    "& .MuiDataGrid-row": { cursor: "pointer" },
  },
});

interface Props {
  items: orch.Composition[] | null;
  showActions: boolean;
  onSelected: (item: orch.Composition) => void;
  onEdit: (item: orch.Composition) => void;
  onDelete: (item: orch.Composition) => void;
}

export default function CompositionsTable({
  items,
  showActions,
  onSelected,
  onEdit,
  onDelete,
}: Props) {
  const classes = useStyles();

  const columns: ColDef[] = useMemo(
    () =>
      !showActions
        ? DATA_COLUMNS
        : [
            ...DATA_COLUMNS,
            {
              field: "",
              headerName: "Actions",
              width: 125,
              disableClickEventBubbling: true,
              sortable: false,
              renderCell: (params: CellParams) => (
                <>
                  <IconButton aria-label="delete" onClick={() => onDelete(params.row as any)}>
                    <DeleteIcon />
                  </IconButton>
                  <IconButton aria-label="edit" onClick={() => onEdit(params.row as any)}>
                    <EditIcon />
                  </IconButton>
                </>
              ),
            },
          ],
    [showActions]
  );

  const onRowClicked = useCallback((params: RowParams) => onSelected(params.row as any), [
    onSelected,
  ]);

  return (
    <DataGrid
      className={classes.compositionsTable}
      loading={!items}
      rows={items ?? []}
      columns={columns}
      autoPageSize
      onRowClick={onRowClicked}
      disableColumnMenu
      disableSelectionOnClick
    />
  );
}
