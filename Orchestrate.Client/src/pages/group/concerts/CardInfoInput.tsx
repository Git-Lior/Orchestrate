import React, { useEffect, useMemo, useState } from "react";
import moment, { Moment } from "moment";
import DateFnsUtils from "@date-io/moment";

import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker,
  KeyboardTimePicker,
} from "@material-ui/pickers";

import TextField from "@material-ui/core/TextField";

import { useInputState } from "utils/hooks";

interface Props {
  concert: orch.Concert | undefined;
  onDataUpdated: React.Dispatch<React.SetStateAction<orch.OptionalId<orch.ConcertData>>>;
}

export default function CardInfoInput({ concert, onDataUpdated }: Props) {
  const [date, setDate] = useState<Moment | null>(concert?.date ? moment.unix(concert.date) : null);
  const [location, setLocation] = useInputState(concert?.location);

  const newUpdatedData: orch.OptionalId<orch.ConcertData> = useMemo(
    () => ({
      date: date?.isValid() ? date.toISOString() : ("" as any),
      location,
    }),
    [date, location]
  );

  useEffect(() => onDataUpdated(u => ({ ...u, ...newUpdatedData })), [
    newUpdatedData,
    onDataUpdated,
  ]);

  return (
    <MuiPickersUtilsProvider utils={DateFnsUtils}>
      <KeyboardDatePicker
        disableToolbar
        helperText=""
        disablePast
        variant="inline"
        label="Date"
        format="DD/MM/yyyy"
        value={date}
        onChange={setDate}
      />
      <KeyboardTimePicker
        disableToolbar
        helperText=""
        initialFocusedDate={moment(0)}
        ampm={false}
        minutesStep={5}
        variant="inline"
        label="Start time"
        value={date}
        onChange={setDate}
      />
      <TextField fullWidth label="Location" value={location} onChange={setLocation} />
    </MuiPickersUtilsProvider>
  );
}