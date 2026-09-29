import { useEffect, useState } from 'react'
import { MenuItem, Stack, TextField } from '@mui/material'
import { DatePicker } from '@mui/x-date-pickers/DatePicker'
import dayjs, { type Dayjs } from 'dayjs'
import { appointmentsApi } from '../api/appointments'
import { formatTime } from '../utils/format'

const isWeekend = (day: Dayjs) => day.day() === 0 || day.day() === 6

/** The first weekday after today. */
function nextWeekday() {
  let day = dayjs().add(1, 'day')
  while (isWeekend(day)) day = day.add(1, 'day')
  return day
}

type Props = {
  slot: string | null
  onSlotChange: (slot: string | null) => void
  onError: (error: unknown) => void
}

/** Date calendar plus a dropdown of the manager's free times on that day. */
export function SlotPicker({ slot, onSlotChange, onError }: Props) {
  const [date, setDate] = useState(nextWeekday)
  const [freeSlots, setFreeSlots] = useState<string[]>([])

  useEffect(() => {
    appointmentsApi.freeSlots(date.format('YYYY-MM-DD')).then(setFreeSlots).catch(onError)
  }, [date, onError])

  function pickDate(day: Dayjs | null) {
    if (!day?.isValid() || isWeekend(day) || !day.isAfter(dayjs(), 'day')) return
    setDate(day)
    onSlotChange(null)
  }

  return (
    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
      <DatePicker
        label="Date"
        value={date}
        onChange={pickDate}
        minDate={dayjs().add(1, 'day')}
        shouldDisableDate={isWeekend}
        sx={{ flex: 1 }}
      />
      <TextField
        select
        label="Time"
        value={slot ?? ''}
        onChange={(e) => onSlotChange(e.target.value)}
        disabled={freeSlots.length === 0}
        helperText={freeSlots.length === 0 ? 'No free times on this day. Please pick another date.' : ' '}
        sx={{ flex: 1 }}
      >
        {freeSlots.map((free) => <MenuItem key={free} value={free}>{formatTime(free)}</MenuItem>)}
      </TextField>
    </Stack>
  )
}
