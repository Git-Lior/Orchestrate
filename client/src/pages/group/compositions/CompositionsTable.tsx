import React, { useCallback } from "react";

import makeStyles from "@material-ui/core/styles/makeStyles";

import { DataGrid, ColDef, RowParams } from "@material-ui/data-grid";

const columns: ColDef[] = [
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
  items: orch.Composition[];
  onSelected: (item: orch.Composition) => void;
}

export default function CompositionsTable({ items, onSelected }: Props) {
  const classes = useStyles();

  const onRowClicked = useCallback((params: RowParams) => onSelected(params.row as any), [
    onSelected,
  ]);

  return (
    <DataGrid
      className={classes.compositionsTable}
      rows={items}
      columns={columns}
      autoPageSize
      onRowClick={onRowClicked}
      disableColumnMenu
      disableSelectionOnClick
    />
  );
}
