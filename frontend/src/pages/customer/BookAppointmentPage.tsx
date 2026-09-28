import { useEffect, useState } from 'react'
import {
  Alert, Box, Button, Card, CardContent, CircularProgress, MenuItem, Stack, TextField, ToggleButton, ToggleButtonGroup, Typography,
} from '@mui/material'
import { DatePicker } from '@mui/x-date-pickers/DatePicker'
import dayjs, { type Dayjs } from 'dayjs'
import { Link as RouterLink, useNavigate } from 'react-router-dom'
import { appointmentsApi } from '../../api/appointments'
import { relationshipsApi } from '../../api/relationships'
import type { AppointmentChannel, Relationship } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { channelLabel, formatDay, formatTime } from '../../utils/format'

const isWeekend = (day: Dayjs) => day.day() === 0 || day.day() === 6

/** The first weekday after today. */
function nextWeekday() {
  let day = dayjs().add(1, 'day')
  while (isWeekend(day)) day = day.add(1, 'day')
  return day
}

export function BookAppointmentPage() {
  const navigate = useNavigate()
  const [relationship, setRelationship] = useState<Relationship | null>()
  const [date, setDate] = useState(nextWeekday)
  const [freeSlots, setFreeSlots] = useState<string[]>([])
  const [slot, setSlot] = useState<string | null>(null)
  const [channel, setChannel] = useState<AppointmentChannel>('Call')
  const [reason, setReason] = useState('')
  const [error, setError] = useState<unknown>(null)
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    relationshipsApi.current().then((current) => setRelationship(current ?? null)).catch(setError)
  }, [])

  const hasManager = relationship?.status === 'Active'

  useEffect(() => {
    if (hasManager) appointmentsApi.freeSlots(date.format('YYYY-MM-DD')).then(setFreeSlots).catch(setError)
  }, [date, hasManager])

  if (relationship === undefined) return error ? <ErrorAlert error={error} /> : <CircularProgress aria-label="Loading" />
  if (!hasManager) return <NoManagerYet relationship={relationship} />

  const { manager, customer } = relationship

  function pickDate(day: Dayjs | null) {
    if (!day?.isValid() || isWeekend(day) || !day.isAfter(dayjs(), 'day')) return
    setDate(day)
    setSlot(null)
  }

  async function confirm() {
    if (!slot) return
    setSubmitting(true)
    setError(null)
    try {
      await appointmentsApi.book({ startsAt: slot, channel, reason })
      navigate('/customer')
    } catch (err) {
      setError(err)
      setSubmitting(false)
    }
  }

  return (
    <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '1fr 340px' }, gap: 4, alignItems: 'start' }}>
      <Stack spacing={2.5}>
        <Typography variant="h1">Book an appointment with {manager.firstName}</Typography>
        <ErrorAlert error={error} />

        <Card>
          <CardContent sx={{ p: 3 }}>
            <Typography sx={{ fontWeight: 700, mb: 2.5 }}>1. When would you like to meet?</Typography>
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
                onChange={(e) => setSlot(e.target.value)}
                disabled={freeSlots.length === 0}
                helperText={freeSlots.length === 0 ? 'No free times on this day. Please pick another date.' : ' '}
                sx={{ flex: 1 }}
              >
                {freeSlots.map((free) => <MenuItem key={free} value={free}>{formatTime(free)}</MenuItem>)}
              </TextField>
            </Stack>
          </CardContent>
        </Card>

        <Card>
          <CardContent sx={{ p: 3 }}>
            <Stack spacing={2}>
              <Typography sx={{ fontWeight: 700 }}>2. How would you like to meet?</Typography>
              <ToggleButtonGroup exclusive fullWidth color="primary" value={channel} onChange={(_, value) => value && setChannel(value)}>
                <ToggleButton value="Call">Call</ToggleButton>
                <ToggleButton value="BranchVisit">Branch visit</ToggleButton>
              </ToggleButtonGroup>
              <Typography color="text.secondary">
                {channel === 'Call'
                  ? `${manager.firstName} will call you on ${customer.phone}`
                  : `Meet at the ${manager.branch} branch`}
              </Typography>
              <TextField
                label="What would you like to discuss?"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                slotProps={{ htmlInput: { maxLength: 200 } }}
              />
            </Stack>
          </CardContent>
        </Card>
      </Stack>

      <Card sx={{ mt: { md: 7 } }}>
        <CardContent sx={{ p: 3 }}>
          <Stack spacing={2}>
            <Typography variant="h3">Summary</Typography>
            <SummaryRow label="When" value={slot ? `${formatDay(slot)}, ${formatTime(slot)}` : 'Pick a time'} />
            <SummaryRow label="How" value={channelLabel(channel)} />
            <SummaryRow label="With" value={`${manager.firstName} ${manager.lastName}`} />
            <SummaryRow label="About" value={reason || '—'} />
            <Button variant="contained" size="large" disabled={!slot || !reason.trim() || submitting} onClick={confirm}>
              {submitting ? 'Booking…' : 'Confirm booking'}
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  )
}

/** Booking needs an active manager: explain why and point to the next step. */
function NoManagerYet({ relationship }: { relationship: Relationship | null }) {
  return (
    <Stack spacing={2.5} sx={{ maxWidth: 560 }}>
      <Typography variant="h1">Book an appointment</Typography>
      <Alert severity="info">
        {relationship
          ? `Your request to ${relationship.manager.firstName} is still pending. You can book once they accept.`
          : 'You need a relationship manager before you can book an appointment.'}
      </Alert>
      <Box>
        <Button variant="contained" component={RouterLink} to="/customer">
          {relationship ? 'View my request' : 'Choose a manager'}
        </Button>
      </Box>
    </Stack>
  )
}

function SummaryRow({ label, value }: { label: string; value: string }) {
  return (
    <Stack direction="row" sx={{ justifyContent: 'space-between', gap: 2 }}>
      <Typography color="text.secondary">{label}</Typography>
      <Typography sx={{ fontWeight: 700, textAlign: 'right' }}>{value}</Typography>
    </Stack>
  )
}
