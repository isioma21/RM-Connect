import { useCallback, useEffect, useState } from 'react'
import { Box, Button, Card, Divider, Stack, Typography } from '@mui/material'
import { appointmentsApi } from '../../api/appointments'
import type { Appointment } from '../../api/types'
import { ConfirmDialog } from '../../components/ConfirmDialog'
import { ErrorAlert } from '../../components/ErrorAlert'
import { StatusChip } from '../../components/StatusChip'
import { RescheduleDialog } from './RescheduleDialog'
import { channelLabel, formatDay, formatTime } from '../../utils/format'

export function MyAppointments() {
  const [appointments, setAppointments] = useState<Appointment[]>([])
  const [error, setError] = useState<unknown>(null)
  const [now] = useState(() => new Date())
  const [toCancel, setToCancel] = useState<Appointment | null>(null)
  const [toReschedule, setToReschedule] = useState<Appointment | null>(null)

  const load = useCallback(() => {
    appointmentsApi.mine().then(setAppointments).catch(setError)
  }, [])

  useEffect(load, [load])

  async function cancel() {
    if (!toCancel) return
    setToCancel(null)
    try {
      await appointmentsApi.cancel(toCancel.id)
      load()
    } catch (err) {
      setError(err)
    }
  }

  const upcoming = appointments.filter((a) => a.status === 'Booked' && new Date(a.startsAt) > now)
  const past = appointments.filter((a) => !upcoming.includes(a)).reverse()

  return (
    <Stack spacing={2}>
      <Typography variant="h2">Appointments</Typography>
      <ErrorAlert error={error} />
      <Card>
        <Section title="Upcoming" appointments={upcoming} empty="No upcoming appointments." onCancel={setToCancel} onReschedule={setToReschedule} />
        <Divider />
        <Section title="Past" appointments={past} empty="No past appointments yet." />
      </Card>
      <ConfirmDialog
        open={toCancel !== null}
        title="Cancel appointment?"
        message={toCancel
          ? `Are you sure you want to cancel your ${channelLabel(toCancel.channel).toLowerCase()} on ${formatDay(toCancel.startsAt)} at ${formatTime(toCancel.startsAt)}?`
          : ''}
        confirmLabel="Yes, cancel appointment"
        onConfirm={cancel}
        onClose={() => setToCancel(null)}
      />
      {toReschedule && (
        <RescheduleDialog
          appointment={toReschedule}
          onClose={() => setToReschedule(null)}
          onRescheduled={() => { setToReschedule(null); load() }}
        />
      )}
    </Stack>
  )
}

type SectionProps = {
  title: string
  appointments: Appointment[]
  empty: string
  onCancel?: (appointment: Appointment) => void
  onReschedule?: (appointment: Appointment) => void
}

function Section({ title, appointments, empty, onCancel, onReschedule }: SectionProps) {
  return (
    <Box>
      <Typography sx={{ px: 3, py: 1.5, fontSize: 13, fontWeight: 700, color: 'text.secondary', bgcolor: '#FAF8F3' }}>
        {title.toUpperCase()}
      </Typography>
      {appointments.length === 0 && <Typography sx={{ px: 3, py: 2.5 }} color="text.secondary">{empty}</Typography>}
      {appointments.map((appointment) => (
        <Stack key={appointment.id} direction="row" spacing={2} sx={{ px: 3, py: 2.5, alignItems: 'center', borderTop: 1, borderColor: 'divider' }}>
          <Box sx={{ flexGrow: 1 }}>
            <Typography sx={{ fontWeight: 700 }}>
              {formatDay(appointment.startsAt)}, {formatTime(appointment.startsAt)} · {channelLabel(appointment.channel)}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {appointment.channel === 'Call'
                ? `${appointment.manager.firstName} will call you on ${appointment.customer.phone}`
                : `${appointment.manager.branch} branch`}
              {' · '}{appointment.reason}
            </Typography>
          </Box>
          <StatusChip status={appointment.status} />
          {onReschedule && <Button variant="outlined" onClick={() => onReschedule(appointment)}>Reschedule</Button>}
          {onCancel && <Button variant="outlined" color="inherit" onClick={() => onCancel(appointment)}>Cancel</Button>}
        </Stack>
      ))}
    </Box>
  )
}
