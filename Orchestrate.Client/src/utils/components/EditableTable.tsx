import React, { useCallback, useMemo, useState } from "react";

import { makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import TextField from "@material-ui/core/TextField";
import IconButton from "@material-ui/core/IconButton";
import DeleteIcon from "@material-ui/icons/Delete";
import EditIcon from "@material-ui/icons/Edit";
import AddIcon from "@material-ui/icons/Add";
import Paper from "@material-ui/core/Paper";
import CircularProgress from "@material-ui/core/CircularProgress";

import { DataGrid, ColDef, CellParams, RowParams, RowModel } from "@material-ui/data-grid";

import { useInputState, usePromiseStatus } from "utils/hooks";
import { AppTheme } from "AppTheme";

import { FormDialog } from "./dialog/FormDialog";
import { ErrorIconTootlip } from "./error/ErrorIconTooltip";

export type { ColDef } from "@material-ui/data-grid";

const useStyles = makeStyles<AppTheme, Props<any>>(theme => ({
  root: { height: "100%" },
  dataGrid: {
    height: "calc(100% - 45px)",
    "& .MuiDataGrid-row": props => ({ cursor: props.onRowClick ? "pointer" : "default" }),
  },
  tableHeader: {
    height: 45,
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    padding: "0 1rem",
    backgroundColor: theme.palette.primary.light,
  },
  tableTitle: { display: "flex", alignItems: "center", "& > *": { marginRight: "2rem" } },
  addRowButton: { cursor: "pointer" },
  cancelEdit: { position: "absolute", left: 0, top: 0 },
  dialogTitle: { marginBottom: "1em", textAlign: "center" },
  doneButton: {},
  dialogStatus: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    overflow: "hidden",
  },
  dialogError: { width: "100%", maxHeight: 125 },
}));

interface DialogProps<T> {
  item: T;
  onFieldChange: <K extends keyof T>(key: K, newValue: T[K]) => void;
}

interface Props<T> {
  rowTypeName: string;
  rows: T[] | undefined;
  emptyRow: orch.OptionalId<T>;
  columns: ColDef[];
  search?: boolean;
  disableActions?: boolean;
  dialogButtons?: React.ReactNode;
  filters?: React.ReactNode;
  onRowClick?: (value: T) => void;
  onRowChange: (value: orch.OptionalId<T>) => Promise<any>;
  onRowDelete: (itemId: number) => Promise<void>;
  children: (props: DialogProps<orch.OptionalId<T>>) => React.ReactNode;
}

export function EditableTable<T extends RowModel>(props: Props<T>) {
  const {
    rowTypeName,
    columns,
    rows,
    emptyRow,
    search,
    disableActions,
    dialogButtons,
    filters,
    onRowDelete,
    onRowChange,
    onRowClick,
    children,
  } = props;
  const classes = useStyles(props);

  const [editedRow, setEditedRow] = useState<orch.OptionalId<T>>();
  const [searchFilter, setSearchFilter] = useInputState();
  const [loading, error, setPromise, clearError] = usePromiseStatus();

  const filteredRows = useMemo(() => {
    if (!rows || !searchFilter) return rows;

    return rows.filter(r =>
      Object.values(r)
        .filter(r => r && (typeof r === "string" || typeof r === "number"))
        .some(r => r.toString().includes(searchFilter))
    );
  }, [searchFilter, rows]);

  const onRowClicked = useCallback((params: RowParams) => onRowClick?.(params.row as any), [
    onRowClick,
  ]);

  const onAddRow = useCallback(() => setEditedRow(emptyRow), [setEditedRow, emptyRow]);

  const onEditCancel = useCallback(() => {
    setEditedRow(undefined);
    clearError();
  }, [clearError]);

  const onEditDone = useCallback(
    () => setPromise(onRowChange(editedRow!).then(() => setEditedRow(undefined))),
    [editedRow, onRowChange, setEditedRow, setPromise]
  );

  const onRowFieldChange = useCallback(
    <K extends keyof orch.OptionalId<T>>(field: K, value: orch.OptionalId<T>[K]) => {
      setEditedRow({ ...editedRow, [field]: value } as any);
    },
    [editedRow, setEditedRow]
  );

  const tableColumns: ColDef[] = useMemo(
    () =>
      disableActions
        ? columns
        : [
            ...columns,
            {
              field: "",
              headerName: "Actions",
              width: 125,
              disableClickEventBubbling: true,
              sortable: false,
              renderCell: (params: CellParams) => (
                <>
                  <IconButton
                    aria-label="delete"
                    onClick={() => !loading && setPromise(onRowDelete(params.row.id as any))}
                  >
                    <DeleteIcon />
                  </IconButton>
                  <IconButton
                    aria-label="edit"
                    onClick={() => {
                      clearError();
                      setEditedRow(params.row as any);
                    }}
                  >
                    <EditIcon />
                  </IconButton>
                </>
              ),
            },
          ],
    [disableActions, columns, loading, onRowDelete, clearError, setPromise]
  );

  return (
    <>
      <Paper className={classes.root}>
        <div className={classes.tableHeader}>
          <div className={classes.tableTitle}>
            <Typography variant="h6" color="secondary">
              {rowTypeName}s
            </Typography>
            {search && (
              <TextField color="secondary" placeholder="search..." onChange={setSearchFilter} />
            )}
            {filters}
          </div>
          {!editedRow && (
            <div>
              {loading && <CircularProgress size="2rem" color="secondary" />}
              {error && <ErrorIconTootlip color="secondary" error={error} />}
            </div>
          )}
          {!disableActions && (
            <AddIcon color="secondary" className={classes.addRowButton} onClick={onAddRow} />
          )}
        </div>
        <div className={classes.dataGrid}>
          <DataGrid
            loading={!rows}
            rows={filteredRows ?? []}
            columns={tableColumns}
            autoPageSize
            onRowClick={onRowClicked}
            disableColumnMenu
            disableSelectionOnClick
          />
        </div>
      </Paper>
      <FormDialog
        open={!!editedRow}
        title={`${!editedRow?.id ? "Create" : "Edit"} ${rowTypeName}`}
        error={error}
        loading={loading}
        dialogButtons={dialogButtons}
        onClose={onEditCancel}
        onDone={onEditDone}
      >
        {children({ item: editedRow!, onFieldChange: onRowFieldChange })}
      </FormDialog>
    </>
  );
}
